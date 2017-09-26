using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Bots;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class BubbleBotCommand : IChatCommand
    {
        public string PermissionRequired => "command_bubble";
        public string Parameters => "[BOTNAME] [BUBBLEID]";
        public string Description => "Utilice una burbuja a medida para charlar.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            string BotName = CommandManager.MergeParams(Params, 1);
            string Bubble = CommandManager.MergeParams(Params, 2);
            int BubbleID = 0;

            if (Params.Length == 1)
            {
                Session.SendWhisper("No has introducido un nombre de bot válido.", 34);
                return;
            }

            RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Params[1]);
            if (Bot == null)
            {
                Bot.Chat("Me acabas de colocar la burbuja " + BubbleID + " iyo.", true, BubbleID);
                Session.SendWhisper("No has introducido un nombre de bot válido.", 34);
                return;
            }

            if (Bot.BotData.ownerID != Session.GetHabbo().Id)
                {
                Session.SendWhisper("Estás cambiándole la burbuja a un bot que no es tuyo.", 34);
                return;
                }

            if (Bubble == "1" || Bubble == "23" || Bubble == "34" || Bubble == "37")
            {
                Session.SendWhisper("Estás colocando una burbuja prohibida.");
                return;
            }

                if (Params.Length == 2)
                {
                    Session.SendWhisper("Uy, se te olvidó introducir una ID de la burbuja.", 34);
                    return;
                }
                
                if (int.TryParse(Bubble, out BubbleID))
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("UPDATE `bots` SET `chat_bubble` =  '" + BubbleID + "' WHERE `name` =  '" + Bot.BotData.Name + "' AND  `room_id` =  '" + Session.GetHabbo().CurrentRoomId + "'");
                        Bot.Chat("Me acabas de colocar la burbuja " + BubbleID + ".", true, BubbleID);
                        Bot.BotData.ChatBubble = BubbleID;
                }
                }

            return;
        }
    }
}