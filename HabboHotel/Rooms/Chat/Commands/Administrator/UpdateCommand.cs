using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Core;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Notifications;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class UpdateCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_update"; }
        }

        public string Parameters
        {
            get { return "%variable%"; }
        }

        public string Description
        {
            get { return "Atualiza o Hotel"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve inserir algo para atualizar");
                return;
            }


            string UpdateVariable = Params[1];
            switch (UpdateVariable.ToLower())
            {
                case "calendar":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Oops, ocorreu um erro");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetCalendarManager().Init();
                        Session.SendWhisper("Calendário atualizado.");
                        break;
                    }
                case "grupos":
                case "groups":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar os grupos.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        QuasarEnvironment.GetGame().GetGroupManager().Init();

                        break;
                    }

                case "cata":
                case "catalog":
                case "catalogue":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o catálogo.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        QuasarEnvironment.GetGame().GetCatalogFrontPageManager().LoadFrontPage();
                        QuasarEnvironment.GetGame().GetCatalog().Init(QuasarEnvironment.GetGame().GetItemManager());
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "O catálogo foi atualizado!", "catalog/open/" + Message + ""));

                        break;
                    }

                case "goals":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o LandingCommunityGoalVS.");
                            break;
                        }

                        string Message = CommandManager.MergeParams(Params, 2);

                        break;
                    }
                case "pinatas":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o premios de las piñatas.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetPinataManager().Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "Os prêmios das piñatas foram atualizados.", ""));
                        break;
                    }

                case "polls":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o premios de las piñatas.");
                            break;
                        }
                        QuasarEnvironment.GetGame().GetPollManager().Init();
                        break;
                    }

                case "list":
                    {
                        StringBuilder List = new StringBuilder("");
                        List.AppendLine ("Lista de comandos para atualizar");
                        List.AppendLine ("---------------------------------");
                        List.AppendLine (":update catalog = Atualiza o catálogo do hotel");
                        List.AppendLine (":update items = Atualiza os items do hotel'");
                        List.AppendLine (":update models = Atualiza os modelos de quarto do hotel");
                        List.AppendLine (":update promotions = Atualiza os avidos da LandingView do hotel");
                        List.AppendLine (":update filter = Atualiza o filtro do hotel");
                        List.AppendLine (":update navigator = Atualiza o navegador do hotel");
                        List.AppendLine (":update rights = Atualiza as permissões do hotel");
                        List.AppendLine (":update configs = Atualiza as configurações do hotel");
                        List.AppendLine (":update bans = Atualiza os usuários banidos do hotel");
                        List.AppendLine (":update tickets = atualiza os pedidos de ajuda do hotel");
                        List.AppendLine (":update badge_definitions = Atualizar os emblemas do hotel");
                        List.AppendLine (":update vouchers = Atualiza os códigos do hotel");
                        List.AppendLine (":update characters = Atualizar as palavras do filtro do hotel");
                        List.AppendLine (":update offers = Atualiza ofertas relâmpago do hotel");
                        List.AppendLine (":update nux = Atualize os prémios nux do hotel");
                        List.AppendLine (":update polls = atualiza as pesquisas do hotel");
                        Session.SendMessage (new MOTDNotificationComposer (List.ToString ()));
                        break;
                    }

                case "nux":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar isso.");
                        break;
                    }

                    QuasarEnvironment.GetGame().GetNuxUserGiftsListManager().Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
                    QuasarEnvironment.GetGame().GetNuxUserGiftsManager().Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
                    Session.SendWhisper("Você recarregou os presentes do nux.");
                    break;

                case "characters":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar os carácteres do filtro");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetFilter().InitCharacters();
                        Session.SendWhisper("Atualizado com sucesso");
                        break;
                    }

                case "items":
                case "furni":
                case "furniture":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o furnis");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetItemManager().Init();
                        Session.SendWhisper("Items atualizado corretamente");
                        break;
                    }

                case "crafting":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar o crafting.");
                        break;
                    }

                    QuasarEnvironment.GetGame().GetCraftingManager().Init();
                    Session.SendWhisper("Crafting atualizado corretamente.");
                    break;

                case "offers":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para atualizar o furnis");
                        break;
                    }

                    QuasarEnvironment.GetGame().GetTargetedOffersManager().Initialize(QuasarEnvironment.GetDatabaseManager().GetQueryReactor());
                    Session.SendWhisper("Ofertas relámpago actualizadas correctamente.");
                    break;

                case "songs":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, você não tem permissão para isso.");
                        break;
                    }

                    QuasarEnvironment.GetGame().GetMusicManager().Init();
                    Session.SendWhisper("Atualizado com sucesso.");
                    break;

                case "models":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_models"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar os Models");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetRoomManager().LoadModels();
                        Session.SendWhisper("Modelos de sala atualizado corretamente");
                        break;
                    }

                case "promotions":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_promotions"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetLandingManager().LoadPromotions();
                        Session.SendWhisper("Atualziado com sucesso.");
                        break;
                    }

                case "youtube":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_youtube"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o videos de Youtube TV.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetTelevisionManager().Init();
                        Session.SendWhisper("Atualizado com sucesso!");
                        break;
                    }

                case "filter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o filtro");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetFilter().InitWords();
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("filters", Session.GetHabbo().Username + " atualizou o filtro do hotel.", ""));
                        break;
                    }

                case "navigator":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_navigator"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para atualizar o navegador.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetNavigator().Init();
                        QuasarEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("newuser", Session.GetHabbo().Username + " alterou os quartos públicos do hotel.", ""));
                        break;
                    }

                case "ranks":
                case "rights":
                case "permissions":
                case "commands":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rights"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetPermissionManager().Init();

                        foreach (GameClient Client in QuasarEnvironment.GetGame().GetClientManager().GetClients.ToList())
                        {
                            if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetPermissions() == null)
                                continue;

                            Client.GetHabbo().GetPermissions().Init(Client.GetHabbo());
                        }

                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + "atualizou as configurações do hotel.", ""));
                        break;
                    }

                case "config":
                case "settings":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_configuration"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso");
                            break;
                        }

                        QuasarEnvironment.ConfigData = new ConfigData();
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " alterou as configurações do hotel.", ""));
                        break;
                    }

                case "bans":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bans"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetModerationManager().ReCacheBans();
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou a lista de banidos do" + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + ".", ""));
                        break;
                    }

                case "quests":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_quests"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetQuestManager().Init();
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou as Quests do hotel " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + ".", ""));
                        break;
                    }

                case "achievements":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_achievements"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetAchievementManager().LoadAchievements();
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou as configurações do hotel.", ""));
                        break;
                    }



                case "moderation":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_moderation"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetModerationManager().Init();
                        QuasarEnvironment.GetGame().GetClientManager().ModAlert("As suas configurações foram alteradas, reentre no hotel para atualizar.");
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " alterou as configurações da moderação.", ""));
                        break;
                    }

                //case "tickets":
                //    {
                //        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_tickets"))
                //        {
                //            Session.SendWhisper("Você não tem permissão para isso.");
                //            break;
                //        }

                //        if (QuasarEnvironment.GetGame().GetModerationTool().Tickets.Count > 0)
                //            QuasarEnvironment.GetGame().GetModerationTool().Tickets.Clear();

                //        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("tickets", Session.GetHabbo().Username + " atualizou os pedidos de ajuda do hotel.", ""));
                //        break;
                //    }

                case "vouchers":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_vouchers"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetCatalog().GetVoucherManager().Init();
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("commandsupdated", Session.GetHabbo().Username + " atualizou os códigos do hotel.", ""));
                        break;
                    }

                case "gc":
                case "games":
                case "gamecenter":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_game_center"))
                        {
                            Session.SendWhisper("Oops, você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetGameDataManager().Init();
                        Session.SendWhisper("Atualizado com sucesso.");
                        break;
                    }

                case "pet_locale":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_pet_locale"))
                        {
                            Session.SendWhisper("Você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetPetLocale().Init();
                        Session.SendWhisper("Atualizado com sucesso");
                        break;
                    }

                case "locale":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_locale"))
                        {
                            Session.SendWhisper("Você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetLanguageLocale().Init();
                        Session.SendWhisper("Atualizado com sucesso");
                        break;
                    }

                case "mutant":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_anti_mutant"))
                        {
                            Session.SendWhisper("Você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetAntiMutant().Init();
                        Session.SendWhisper("Atualziado com sucesso");
                        break;
                    }

                case "bots":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bots"))
                        {
                            Session.SendWhisper("Você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetBotManager().Init();
                        Session.SendWhisper("Atualizado com sucesso");
                        break;
                    }

                case "rewards":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                        {
                            Session.SendWhisper("Você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetRewardManager().Reload();
                        Session.SendWhisper("Atualizado com sucesso");
                        break;
                    }

                case "chat_styles":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_chat_styles"))
                        {
                            Session.SendWhisper("Você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetChatManager().GetChatStyles().Init();
                        Session.SendWhisper("Atualizado com sucesso");
                        break;
                    }

                case "badges":
                case "badge_definitions":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_badge_definitions"))
                        {
                            Session.SendWhisper("Você não tem permissão para isso.");
                            break;
                        }

                        QuasarEnvironment.GetGame().GetBadgeManager().Init();
                        QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("definitions", Session.GetHabbo().Username + " atualizou os emblemas do hotel.", ""));
                        break;
                    }
                default:
                    Session.SendWhisper("'" + UpdateVariable + "' é um comando invalido");
                    break;
            }
        }
    }
}
