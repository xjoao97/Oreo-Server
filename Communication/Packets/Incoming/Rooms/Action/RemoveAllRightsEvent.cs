using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Communication.Packets.Outgoing.Rooms.Settings;
using Quasar.Communication.Packets.Outgoing.Rooms.Permissions;
using Quasar.Database.Interfaces;


namespace Quasar.Communication.Packets.Incoming.Rooms.Action
{
    class RemoveAllRightsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Instance;

            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Instance))
                return;

            if (!Instance.CheckRights(Session, true))
                return;

            foreach (int UserId in new List<int>(Instance.UsersWithRights))
            {
                RoomUser User = Instance.GetRoomUserManager().GetRoomUserByHabbo(UserId);
                if (User != null && !User.IsBot)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendMessage(new YouAreControllerComposer(0));
                }

                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `room_rights` WHERE `user_id` = @uid AND `room_id` = @rid LIMIT 1");
                    dbClient.AddParameter("uid", UserId);
                    dbClient.AddParameter("rid", Instance.Id);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new FlatControllerRemovedComposer(Instance, UserId));
                Session.SendMessage(new RoomRightsListComposer(Instance));
                Session.SendMessage(new UserUpdateComposer(Instance.GetRoomUserManager().GetUserList()));
            }

            if (Instance.UsersWithRights.Count > 0)
                Instance.UsersWithRights.Clear();
        }
    }
}
