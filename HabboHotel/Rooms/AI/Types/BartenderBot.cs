using System;
using System.Drawing;

using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms.AI.Speech;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.Rooms.AI.Responses;
using Quasar.Utilities;

namespace Quasar.HabboHotel.Rooms.AI.Types
{
    class BartenderBot : BotAI
    {
        private int VirtualId;
        private int ActionTimer = 0;
        private int SpeechTimer = 0;

        public BartenderBot(int VirtualId)
        {
            this.VirtualId = VirtualId;
        }

        public override void OnSelfEnterRoom()
        {

        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
        }

        public override void OnUserLeaveRoom(GameClient Client)
        {

        }

        public override void OnUserSay(RoomUser User, string Message)
        {
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return;

            if (Gamemap.TileDistance(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y) > 8)
                return;

            BotResponse Response = QuasarEnvironment.GetGame().GetBotManager().GetResponse(GetBotData().AiType, Message);
            if (Response == null)
                return;

            switch (Response.ResponseType.ToLower())
            {
                case "say":
                    GetRoomUser().Chat(Response.ResponseText.Replace("{username}", User.GetClient().GetHabbo().Username), false);
                    break;

                case "shout":
                    GetRoomUser().Chat(Response.ResponseText.Replace("{username}", User.GetClient().GetHabbo().Username), true);
                    break;

                case "whisper":
                    User.GetClient().SendMessage(new WhisperComposer(GetRoomUser().VirtualId, Response.ResponseText.Replace("{username}", User.GetClient().GetHabbo().Username), 0, 0));
                    break;
            }

            if (Response.BeverageIds.Count > 0)
            {
                User.CarryItem(Response.BeverageIds[RandomNumber.GenerateRandom(0, (Response.BeverageIds.Count - 1))]);
            }
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return;

            if (Gamemap.TileDistance(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y) > 8)
                return;

            BotResponse Response = QuasarEnvironment.GetGame().GetBotManager().GetResponse(GetBotData().AiType, Message);
            if (Response == null)
                return;

            switch (Response.ResponseType.ToLower())
            {
                case "say":
                    GetRoomUser().Chat(Response.ResponseText.Replace("{username}", User.GetClient().GetHabbo().Username), false);
                    break;

                case "shout":
                    GetRoomUser().Chat(Response.ResponseText.Replace("{username}", User.GetClient().GetHabbo().Username), true);
                    break;

                case "whisper":
                    User.GetClient().SendMessage(new WhisperComposer(GetRoomUser().VirtualId, Response.ResponseText.Replace("{username}", User.GetClient().GetHabbo().Username), 0, 0));
                    break;
            }

            if (Response.BeverageIds.Count > 0)
            {
                User.CarryItem(Response.BeverageIds[RandomNumber.GenerateRandom(0, (Response.BeverageIds.Count - 1))]);
            }
        }

        public override void OnTimerTick()
        {
            if (GetBotData() == null)
                return;

            if (SpeechTimer <= 0)
            {
                if (GetBotData().RandomSpeech.Count > 0)
                {
                    if (GetBotData().AutomaticChat == false)
                        return;

                    RandomSpeech Speech = GetBotData().GetRandomSpeech();

                    string word;
                    string String = QuasarEnvironment.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Speech.Message, out word) ? "Spam" : Speech.Message;
                    if (String.Contains("<img src") || String.Contains("<font ") || String.Contains("</font>") || String.Contains("</a>") || String.Contains("<i>"))
                        String = "I really shouldn't be using HTML within bot speeches.";
                    GetRoomUser().Chat(String, false, GetBotData().ChatBubble);
                }
                SpeechTimer = GetBotData().SpeakingInterval;
            }
            else
                SpeechTimer--;

            if (ActionTimer <= 0)
            {
                Point nextCoord;
                switch (GetBotData().WalkingMode.ToLower())
                {
                    default:
                    case "stand":

                        break;

                    case "freeroam":
                        if (GetBotData().ForcedMovement)
                        {
                            if (GetRoomUser().Coordinate == GetBotData().TargetCoordinate)
                            {
                                GetBotData().ForcedMovement = false;
                                GetBotData().TargetCoordinate = new Point();

                                GetRoomUser().MoveTo(GetBotData().TargetCoordinate.X, GetBotData().TargetCoordinate.Y);
                            }
                        }
                        else if (GetBotData().ForcedUserTargetMovement > 0)
                        {
                            RoomUser Target = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().ForcedUserTargetMovement);
                            if (Target == null)
                            {
                                GetBotData().ForcedUserTargetMovement = 0;
                                GetRoomUser().ClearMovement(true);
                            }
                            else
                            {
                                var Sq = new Point(Target.X, Target.Y);

                                if (Target.RotBody == 0)
                                {
                                    Sq.Y--;
                                }
                                else if (Target.RotBody == 2)
                                {
                                    Sq.X++;
                                }
                                else if (Target.RotBody == 4)
                                {
                                    Sq.Y++;
                                }
                                else if (Target.RotBody == 6)
                                {
                                    Sq.X--;
                                }


                                GetRoomUser().MoveTo(Sq);
                            }
                        }
                        else if (GetBotData().TargetUser == 0)
                        {
                            nextCoord = GetRoom().GetGameMap().getRandomWalkableSquare();
                            GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        }
                        break;

                    case "specified_range":

                        break;
                }

                ActionTimer = new Random(DateTime.Now.Millisecond + this.VirtualId ^ 2).Next(5, 15);
            }
            else
                ActionTimer--;
        }
    }
}
