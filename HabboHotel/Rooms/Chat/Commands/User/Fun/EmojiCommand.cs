using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class EmojiCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_emoji"; }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get { return "Use emoji para interagir"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Oops, moji invalido digite :emoji list para ver quais você tem disponível!");
                return;
            }
            string emoji = Params[1];

            if (emoji.Equals("list"))
            {
                Session.SendMessage(new MassEventComposer("habbopages/chat/emoji.txt"));
            }
            else
            {
                int emojiNum;
                bool isNumeric = int.TryParse(emoji, out emojiNum);
                if (isNumeric)
                {
                    string chatcolor = Session.GetHabbo().chatHTMLColour;
                    int chatsize = Session.GetHabbo().chatHTMLSize;

                    Session.GetHabbo().chatHTMLColour = "";
                    Session.GetHabbo().chatHTMLSize = 12;
                    switch (emojiNum)
                    {
                        default:
                            bool isValid = true;
                            if (emojiNum < 1)
                            {
                                isValid = false;
                            }

                            if (emojiNum > 189 && Session.GetHabbo().Rank < 6)
                            {
                                isValid = false;
                            }
                            if (isValid)
                            {
                                string Username;
                                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
                                if (emojiNum < 10)
                                {
                                    Username = "<img src='emoji/habbo/" + emojiNum + ".png' height='16' width='16'><br>       ";
                                }
                                else
                                {
                                    Username = "<img src='emoji/habbo/" + emojiNum + ".png' height='16' width='16'><br>       ";
                                }
                                if (Room != null)
                                    Room.SendMessage(new UserNameChangeComposer(Session.GetHabbo().CurrentRoomId, TargetUser.VirtualId, Username));

                                string Message = string.Empty;
                                    Room.SendMessage(new ChatComposer(TargetUser.VirtualId, Message, 0, TargetUser.LastBubble));
                                TargetUser.SendNamePacket();

                            }
                            else
                            {
                                Session.SendWhisper("Emoji invalido, digite :emoji list para ver quais você tem disponível!");
                            }

                            break;
                    }
                    Session.GetHabbo().chatHTMLColour = chatcolor;
                    Session.GetHabbo().chatHTMLSize = chatsize;
                }
                else
                {
                    Session.SendWhisper("Emoji invalido, digite :emoji list para ver quais você tem disponível!");
                }
            }
            return;
        }
    }
}
