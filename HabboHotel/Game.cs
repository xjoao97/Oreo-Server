using log4net;
using System;
using Oreo.Communication.Packets;
using Oreo.HabboHotel.GameClients;
using Oreo.HabboHotel.Moderation;
using Oreo.HabboHotel.Catalog;
using Oreo.HabboHotel.Items;
using Oreo.HabboHotel.Items.Crafting;
using Oreo.HabboHotel.Items.Televisions;
using Oreo.HabboHotel.Navigator;
using Oreo.HabboHotel.Rooms;
using Oreo.HabboHotel.Groups;
using Oreo.HabboHotel.Groups.Forums;
using Oreo.HabboHotel.Quests;
using Oreo.HabboHotel.Achievements;
using Oreo.HabboHotel.LandingView;
using Oreo.HabboHotel.Games;
using Oreo.HabboHotel.Rooms.Chat;
using Oreo.HabboHotel.Bots;
using Oreo.HabboHotel.Cache;
using Oreo.HabboHotel.Rewards;
using Oreo.HabboHotel.Badges;
using Oreo.HabboHotel.Permissions;
using Oreo.HabboHotel.Subscriptions;
using Oreo.HabboHotel.Rooms.TraxMachine;
using Oreo.HabboHotel.Rooms.Camera;
using Oreo.HabboHotel.Helpers;
using System.Threading;
using Oreo.Database.Interfaces;
using Oreo.Core;
using Oreo.HabboHotel.Rooms.Polls;
using Oreo.Core.FigureData;
using Oreo.Core.Settings;
using Oreo.Core.Language;
using Oreo.HabboHotel.Catalog.FurniMatic;
using Oreo.Communication.Packets.Incoming.LandingView;

namespace Oreo.HabboHotel
{
    public class Game
    {
        private static readonly ILog log = LogManager.GetLogger("Oreo.HabboHotel.Game");
        internal bool ClientManagerCycleEnded, RoomManagerCycleEnded;
        private readonly PacketManager _packetManager;
        private readonly GameClientManager _clientManager;
        private readonly ModerationManager _moderationManager;
        private readonly ItemDataManager _itemDataManager;
        private readonly CatalogManager _catalogManager;
        private readonly TelevisionManager _televisionManager;//TODO: Initialize from the item manager.
        private readonly NavigatorManager _navigatorManager;
        private readonly RoomManager _roomManager;
        private readonly ChatManager _chatManager;
        private readonly GroupManager _groupManager;
        private readonly GroupForumManager _groupForumManager;
        private readonly QuestManager _questManager;
        private readonly AchievementManager _achievementManager;
        private readonly LandingViewManager _landingViewManager;
        private readonly GameDataManager _gameDataManager;
        private readonly BotManager _botManager;
        private readonly CacheManager _cacheManager;
        private readonly RewardManager _rewardManager;
        private readonly BadgeManager _badgeManager;
        private readonly PermissionManager _permissionManager;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly TargetedOffersManager _targetedoffersManager;
        private readonly CrackableManager _crackableManager;
        private readonly FurniMaticRewardsManager _furniMaticRewardsManager;
        private readonly FigureDataManager _figureManager;
        private readonly LanguageManager _languageManager;
        private readonly SettingsManager _settingsManager;
        private readonly CraftingManager _craftingManager;
		private readonly PollManager _pollManager;
		private Thread _gameLoop;
        internal static bool GameLoopEnabled = true;
        public static int SessionUserRecord;
        internal bool GameLoopActiveExt { get; private set; }

        public Game()
        {
            Console.WriteLine();
            log.Info("Estamos iniciando o Oreo Server para  " + OreoServer.HotelName + "");
            Console.WriteLine();

            SessionUserRecord = 0;
            // Run Extra Settings
           // BotFrankConfig.RunBotFrank();
            ExtraSettings.RunExtraSettings();

            // Run Catalog Settings
            CatalogSettings.RunCatalogSettings();

            // Run Notification Settings
            NotificationSettings.RunNotiSettings();


			_languageManager = new LanguageManager();
            _languageManager.Init();

            _settingsManager = new SettingsManager();
            _settingsManager.Init();

            _packetManager = new PacketManager();
            _clientManager = new GameClientManager();

            _moderationManager = new ModerationManager();
            _moderationManager.Init();

            _itemDataManager = new ItemDataManager();
            _itemDataManager.Init();

            _catalogManager = new CatalogManager();
            _catalogManager.Init(_itemDataManager);

            _craftingManager = new CraftingManager();
            _craftingManager.Init();

            _televisionManager = new TelevisionManager();

            _navigatorManager = new NavigatorManager();
            _roomManager = new RoomManager();
            _chatManager = new ChatManager();
            _groupManager = new GroupManager();
            _groupManager.Init();
            _groupForumManager = new GroupForumManager();
            _questManager = new QuestManager();
            _achievementManager = new AchievementManager();
            _landingViewManager = new LandingViewManager();
            _gameDataManager = new GameDataManager();

            _botManager = new BotManager();
           
            _cacheManager = new CacheManager();
            _rewardManager = new RewardManager();

            _badgeManager = new BadgeManager();
            _badgeManager.Init();

            GetHallOfFame.GetInstance().Load();

            _permissionManager = new PermissionManager();
            _permissionManager.Init();

            _subscriptionManager = new SubscriptionManager();
            _subscriptionManager.Init();

            TraxSoundManager.Init();
            HabboCameraManager.Init();
            HelperToolsManager.Init();

            _figureManager = new FigureDataManager(OreoServer.GetConfig().data["game.legacy.figure_mutant"].ToString() == "1");
            _figureManager.Init();

            _crackableManager = new CrackableManager();
            _crackableManager.Initialize(OreoServer.GetDatabaseManager().GetQueryReactor());

            _furniMaticRewardsManager = new FurniMaticRewardsManager();
            _furniMaticRewardsManager.Initialize(OreoServer.GetDatabaseManager().GetQueryReactor());

            _targetedoffersManager = new TargetedOffersManager();
            _targetedoffersManager.Initialize(OreoServer.GetDatabaseManager().GetQueryReactor());
        }

        public void ContinueLoading()
        {
            ServerStatusUpdater.Init();
			StartGameLoop();
        }

        public void StartGameLoop()
        {
            GameLoopActiveExt = true;
			_gameLoop = new Thread(GameCycle)
			{
				Name = "Game Loop"
			};
			_gameLoop.Start();

        }

        public PollManager GetPollManager()
        {
            return _pollManager;
        }

        private void GameCycle()
        {
            ServerStatusUpdater.StartProcessing();

            while (GameLoopActiveExt)
            {
                if (GameLoopEnabled)
                    try
                    {
                        RoomManagerCycleEnded = false;
                        ClientManagerCycleEnded = false;
                        _roomManager.OnCycle();
                        _clientManager.OnCycle();
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogCriticalException(ex);
                    }
                Thread.Sleep(25);
            }
        }

        public void StopGameLoop()
        {
            GameLoopActiveExt = false;
            while (!RoomManagerCycleEnded || !ClientManagerCycleEnded) Thread.Sleep(25);
        }



        public PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public GameClientManager GetClientManager()
        {
            return _clientManager;
        }

        public CatalogManager GetCatalog()
        {
            return _catalogManager;
        }

        public NavigatorManager GetNavigator()
        {
            return _navigatorManager;
        }

        public ItemDataManager GetItemManager()
        {
            return _itemDataManager;
        }

        public RoomManager GetRoomManager()
        {
            return _roomManager;
        }

        internal TargetedOffersManager GetTargetedOffersManager()
        {
            return _targetedoffersManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return _achievementManager;
        }

        public ModerationManager GetModerationManager()
        {
            return _moderationManager;
        }

        public PermissionManager GetPermissionManager()
        {
            return _permissionManager;
        }

        public SubscriptionManager GetSubscriptionManager()
        {
            return _subscriptionManager;
        }

        public QuestManager GetQuestManager()
        {
            return _questManager;
        }

        public GroupManager GetGroupManager()
        {
            return _groupManager;
        }

        public GroupForumManager GetGroupForumManager()
        {
            return _groupForumManager;
        }

        public LandingViewManager GetLandingManager()
        {
            return _landingViewManager;
        }
        public TelevisionManager GetTelevisionManager()
        {
            return _televisionManager;
        }

        public ChatManager GetChatManager()
        {
            return _chatManager;
        }

        public FurniMaticRewardsManager GetFurniMaticRewardsMnager()
        {
            return _furniMaticRewardsManager;
        }

        internal CrackableManager GetPinataManager()
        {
            return _crackableManager;
        }

        public GameDataManager GetGameDataManager()
        {
            return _gameDataManager;
        }

        public BotManager GetBotManager()
        {
            return _botManager;
        }

        public CacheManager GetCacheManager()
        {
            return _cacheManager;
        }

        public LanguageManager GetLanguageManager()
        {
            return _languageManager;
        }

        public SettingsManager GetSettingsManager()
        {
            return _settingsManager;
        }

        public CraftingManager GetCraftingManager()
        {
            return _craftingManager;
        }

        public FigureDataManager GetFigureManager()
        {
            return _figureManager;
        }

        public RewardManager GetRewardManager()
        {
            return _rewardManager;
        }

        public BadgeManager GetBadgeManager()
        {
            return _badgeManager;
        }
    }
}