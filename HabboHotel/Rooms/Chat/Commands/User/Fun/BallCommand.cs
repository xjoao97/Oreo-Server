using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quasar.Database.Interfaces;

using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fan
{
    class BallCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_shoot"; }
        }
        public string Parameters
        {
            get { return "%value%"; }
        }
        public string Description
        {
            get { return "Altera o valor de quadros que a bola corre!"; }
        }
        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva um número de 1 a 4. (0 Desativa o DoubleClick)");
                return;
            }

            int Value;
            if (int.TryParse(Params[1], out Value))
            {
                if (Value < 0 || Value > 4)
                {
                    Session.SendWhisper("Número ivalido!");
                    return;
                }

                Room.Shoot = Value;
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `rooms` SET `shoot` = @Shoot WHERE `id` = '" + Room.Id + "' LIMIT 1");
                    dbClient.AddParameter("Shoot", Convert.ToInt32(Room.Shoot));
                    dbClient.RunQuery();
                }

                Session.SendWhisper("Bola definida para: " + Room.Shoot + "");
            }
        }
    }
}
