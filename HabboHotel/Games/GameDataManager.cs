using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;

using Quasar.Database.Interfaces;
using log4net;


namespace Quasar.HabboHotel.Games
{
    public class GameDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("Quasar.HabboHotel.Games.GameDataManager");

        private readonly Dictionary<int, GameData> _games;

        public GameDataManager()
        {
            this._games = new Dictionary<int, GameData>();

            this.Init();
        }

        public void Init()
        {
            GameDataDao.LoadGameData(_games);
        }

        public bool TryGetGame(int GameId, out GameData GameData)
        {
            if (this._games.TryGetValue(GameId, out GameData))
                return true;
            return false;
        }

        public int GetCount()
        {
            int GameCount = 0;
            foreach (GameData Game in this._games.Values)
            {
                if (Game.GameEnabled)
                    GameCount += 1;
            }
            return GameCount;
        }

        public ICollection<GameData> GameData
        {
            get
            {
                return this._games.Values;
            }
        }
    }
}
