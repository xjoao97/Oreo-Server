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
        public string Parameters => "[BOT] [BUBBLEID]";
        public string Description => "Use um balão de fala personalizado para seu bot.";

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
                Session.SendWhisper("Você não inseriu um nome de bot válido.", 34);
                return;
            }

            RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Params[1]);
            if (Bot == null)
            {
                Bot.Chat("Você acabou de colocar o balão de fala " + BubbleID + " no bot.", true, BubbleID);
                Session.SendWhisper("Você não inseriu um nome de bot válido.", 34);
                return;
            }

            if (Bot.BotData.ownerID != Session.GetHabbo().Id)
                {
                Session.SendWhisper("Você está mudando o balão para um bot que não é seu.", 34);
                return;
                }

            if (Bubble == "1" || Bubble == "23" || Bubble == "34" || Bubble == "37")
            {
                Session.SendWhisper("Você está colocando um balão proibida.");
                return;
            }

                if (Params.Length == 2)
                {
                    Session.SendWhisper("Oops, você esqueceu de escolher o ID do balão", 34);
                    return;
                }

                if (int.TryParse(Bubble, out BubbleID))
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("UPDATE `bots` SET `chat_bubble` =  '" + BubbleID + "' WHERE `name` =  '" + Bot.BotData.Name + "' AND  `room_id` =  '" + Session.GetHabbo().CurrentRoomId + "'");
                        Bot.Chat("Você escolheu o balão " + BubbleID + ".", true, BubbleID);
                        Bot.BotData.ChatBubble = BubbleID;
                }
                }

            return;
        }
    }
}
