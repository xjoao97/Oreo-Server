using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class LandingCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_landing"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Comando de segurança"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            GameClient Target = QuasarEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("A LandingView foi atualizada com sucesso!");
                return;
            }

            string RankType = Params[2];
            switch (RankType.ToLower())
            {
                case "adm":
                case "admin":
                    {
                        if (Session.GetHabbo().Rank < 1)
                        {
                            Session.SendWhisper("Oops, você não possui as permissões necessárias para usar este comando!");
                            break;
                        }
                        else
                        {
                            int Rank;
                            if (int.TryParse(Params[3], out Rank))
                            {
                                byte RankByte = Convert.ToByte(Rank);

                                if(Rank > 9 || Rank < 0)
                                {
                                    Session.SendWhisper("Você não pode colocar um rank inferior a 9.", 1);
                                    return;
                                }

                                Target.GetHabbo()._guidelevel = RankByte;

                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                { dbClient.RunQuery("UPDATE `users` SET `rank` = '" + RankByte + "' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1"); }
                                switch (RankByte)
                                {
                                    case 0:
                                        Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " alterou o seu cargo reentre no hotel para aplicar as novas atualizações!", ""));
                                        Session.SendWhisper("Você adicionou o usuário(a) " + Target.GetHabbo().Username + " a equipe.");
                                        Target.Disconnect();
                                        break;
                                }

                                break;
                            }

                            else { Session.SendWhisper("Oops, " + RankType + " digite um valor válido."); break; }
                        }
                    }

                case "pin":
                    {
                        if (Session.GetHabbo().Rank < 1)
                        {
                            Session.SendWhisper("Oops, você não possui as permissões necessárias para usar este comando!");
                            break;
                        }

                        else
                        {
                            int Rank;
                            if (int.TryParse(Params[3], out Rank))
                            {
                                byte RankByte = Convert.ToByte(Rank);

                                if (Rank > 9 || Rank < 0)
                                { Session.SendWhisper("Você deve escolher um valor de maior que 0.", 1); return; }

                                Target.GetHabbo()._publicistalevel = RankByte;
                                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                                { dbClient.RunQuery("UPDATE `users` SET `pin_client` = '" + RankByte + "' WHERE `id` = '" + Target.GetHabbo().Id + "' LIMIT 1"); }

                                switch (RankByte)
                                {
                                   case 0:
                                   Target.SendMessage(RoomNotificationComposer.SendBubble("eventoxx", Session.GetHabbo().Username + " pin adicionado com sucesso!", ""));
                                   Session.SendWhisper("Você alterou o pin do usuário(a) " + Target.GetHabbo().Username + "");
                                   Target.Disconnect();
                                   break;
                                }
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, " + Rank + " digitre um valor válido.");
                                break;
                            }
                        }
                    }

            }
        }
    }
}
