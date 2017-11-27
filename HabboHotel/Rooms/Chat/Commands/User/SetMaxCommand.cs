using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class SetMaxCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_setmax"; }
        }

        public string Parameters
        {
            get { return "%límite%"; }
        }

        public string Description
        {
            get { return "Aumente ou reduza a capacidade máxima em seu quarto."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite um número correto (em números) de quantos usuários podem entrar no seu quarto.");
                return;
            }

            int MaxAmount;
            if (int.TryParse(Params[1], out MaxAmount))
            {
                if (MaxAmount <= 0)
                {
                    MaxAmount = 10;
                    Session.SendWhisper("Quantidade de visitantes muito baixa, o número de visitantes foi definido para 10.");
                }
                else if (MaxAmount > 250 && !Session.GetHabbo().GetPermissions().HasRight("override_command_setmax_limit"))
                {
                    MaxAmount = 250;
                    Session.SendWhisper("A quantidade de visitantes é muito alta, o número de visitantes foi fixado em 250.");
                }
                else

                Room.UsersMax = MaxAmount;
                Room.RoomData.UsersMax = MaxAmount;
                Room.SendMessage(RoomNotificationComposer.SendBubble("setmax", "" + Session.GetHabbo().Username + " definiu o limite de visitantes no quarto para " + MaxAmount + ".", ""));

                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `rooms` SET `users_max` = " + MaxAmount + " WHERE `id` = '" + Room.Id + "' LIMIT 1");
                }
            }
            else
                Session.SendWhisper("Valor invalido, apenas os números são permitidos.");
        }
    }
}
