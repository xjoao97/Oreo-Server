using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;



using Quasar.Communication.Packets.Outgoing.Rooms.Engine;

using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class RoomCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_room"; }
        }

        public string Parameters
        {
            get { return "respect/pets"; }
        }

        public string Description
        {
            get { return "Ligue ou desligue estás funções no seu quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops,Você deve escolher uma opção para desativar, digite :room list'");
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Oops, apenas o proprietário da sala pode executar este comando");
                return;
            }

            string Option = Params[1];
            switch (Option)
            {
                case "list":
                {
                    StringBuilder List = new StringBuilder("");
                    List.AppendLine("Lista de comandos da sala");
                    List.AppendLine("-------------------------");
                    List.AppendLine("Pet Morphs: " + (Room.PetMorphsAllowed == true ? "Habilitado" : "Desabilitado"));
                    List.AppendLine("Pull: " + (Room.PullEnabled == true ? "Habilitado" : "Desabilitado"));
                    List.AppendLine("Push: " + (Room.PushEnabled == true ? "Habilitado" : "Desabilitado"));
                    List.AppendLine("Super Pull: " + (Room.SPullEnabled == true ? "Habilitado" : "Desabilitado"));
                    List.AppendLine("Super Push: " + (Room.SPushEnabled == true ? "Habilitado" : "Desabilitado"));
                    List.AppendLine("Respeitos: " + (Room.RespectNotificationsEnabled == true ? "Habilitado" : "Desabilitado"));
                    List.AppendLine("Enables: " + (Room.EnablesEnabled == true ? "Habilitado" : "Desabilitado"));
                    Session.SendNotification(List.ToString());
                    break;
                }

                case "push":
                    {
                        Room.PushEnabled = !Room.PushEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `push_enabled` = @PushEnabled WHERE `id` = '" + Room.Id +"' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", QuasarEnvironment.BoolToEnum(Room.PushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Push agora está " + (Room.PushEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "spush":
                    {
                        Room.SPushEnabled = !Room.SPushEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spush_enabled` = @PushEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PushEnabled", QuasarEnvironment.BoolToEnum(Room.SPushEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Super Push agora está " + (Room.SPushEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "spull":
                    {
                        Room.SPullEnabled = !Room.SPullEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `spull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", QuasarEnvironment.BoolToEnum(Room.SPullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Super Pull agora está  " + (Room.SPullEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "pull":
                    {
                        Room.PullEnabled = !Room.PullEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pull_enabled` = @PullEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PullEnabled", QuasarEnvironment.BoolToEnum(Room.PullEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Modo Pull agora está " + (Room.PullEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "enable":
                case "enables":
                    {
                        Room.EnablesEnabled = !Room.EnablesEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `enables_enabled` = @EnablesEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("EnablesEnabled", QuasarEnvironment.BoolToEnum(Room.EnablesEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Os efeitos nesta sala estão " + (Room.EnablesEnabled == true ? "Habilitados!" : "Desabilitados!"));
                        break;
                    }

                case "respect":
                    {
                        Room.RespectNotificationsEnabled = !Room.RespectNotificationsEnabled;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `respect_notifications_enabled` = @RespectNotificationsEnabled WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("RespectNotificationsEnabled", QuasarEnvironment.BoolToEnum(Room.RespectNotificationsEnabled));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("As notificações de respeito estão " + (Room.RespectNotificationsEnabled == true ? "Habilitado!" : "Desabilitado!"));
                        break;
                    }

                case "pets":
                case "morphs":
                    {
                        Room.PetMorphsAllowed = !Room.PetMorphsAllowed;
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `rooms` SET `pet_morphs_allowed` = @PetMorphsAllowed WHERE `id` = '" + Room.Id + "' LIMIT 1");
                            dbClient.AddParameter("PetMorphsAllowed", QuasarEnvironment.BoolToEnum(Room.PetMorphsAllowed));
                            dbClient.RunQuery();
                        }

                        Session.SendWhisper("Se transformar em mascotes está " + (Room.PetMorphsAllowed == true ? "Habilitado!" : "Desabilitado!"));

                        if (!Room.PetMorphsAllowed)
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
                            {
                                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                                    continue;

                                User.GetClient().SendWhisper("O proprietário da sala desativou a opção de se tornar um animal de estimação.");
                                if (User.GetClient().GetHabbo().PetId > 0)
                                {
                                    User.GetClient().SendWhisper("Oops, O dono da sala só permite usuários normais, sem animais de estimação.");
                                    User.GetClient().GetHabbo().PetId = 0;
                                    Room.SendMessage(new UserRemoveComposer(User.VirtualId));
                                    Room.SendMessage(new UsersComposer(User));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
