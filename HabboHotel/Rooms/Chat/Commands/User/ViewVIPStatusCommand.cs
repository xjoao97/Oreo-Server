using Quasar.HabboHotel.GameClients;

using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Database.Interfaces;
using System.Data;
using System;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class ViewVIPStatusCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_info"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Informação do Servidor."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            //string Name;
            //RoomData Data = QuasarEnvironment.GetGame().GetRoomManager().GenerateRoomData(Session.GetHabbo().CurrentRoomId);
            //if (Data == null)
            //    return;

            //if (Data.OwnerId != Session.GetHabbo().Id)
            //    return;

            //if (Data.Promotion == null)
            //    Data.Promotion = new RoomPromotion("xD", "xD2", 16);
            //else
            //{
            //    Data.Promotion.Name = "XD";
            //    Data.Promotion.Description = "XD2";
            //    Data.Promotion.TimestampExpires += 7200;
            //}

            //using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            //{
            //    dbClient.SetQuery("SELECT * FROM 'user_subscriptions' WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND 'subscription_id' = 'club_vip'");
            //    DataRow Row = dbClient.getRow();

            //    if (Row != null)
            //    {
            //        int Expire = Convert.ToInt32(Row["timestamp_expire"]);
            //        Console.WriteLine("hla");
            //        DateTime valecrack = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            //        valecrack = valecrack.AddSeconds(Expire).ToLocalTime();

            //        string time = valecrack.ToString();

            //        Console.WriteLine(time);

            //        Session.SendMessage(RoomNotificationComposer.SendBubble("cred", "" + time + " te acaba de enviar  créditos.", ""));
            //    }
            //}

                //Session.GetHabbo().CurrentRoom.SendMessage(new RoomEventComposer(Data, Data.Promotion));
        }
    }
}
