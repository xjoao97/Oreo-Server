using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class PollCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            int time = int.Parse(Params[1]);
            string quest = CommandManager.MergeParams(Params, 2);

            if (Params.Length == 0)
            {
                Session.SendWhisper("Por favor introduce la pregunta");
            }
            else
            {
                
                if (quest == "end")
                {
                    Room.endQuestion();
                }
                else if(time != -1 || time != 0)
                {
                    Room.startQuestion(quest);
                    time = time * 1000;
                    Task t = Task.Factory.StartNew(() => TaskStopQuestion(Room, time));
                }
                else
                    Room.startQuestion(quest);

            }
        }

        public void TaskStopQuestion(Room room, int time)
        {
            Thread.Sleep(time);
            room.endQuestion();
        }

        public string Description =>
            "Realizar una encuesta rápida.";

        public string Parameters =>
            "%time% %question%";

        public string PermissionRequired =>
            "command_give_badge";
    }
}