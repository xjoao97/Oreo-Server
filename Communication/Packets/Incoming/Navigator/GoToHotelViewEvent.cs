﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Rooms.Session;
using Quasar.HabboHotel.Users;

namespace Quasar.Communication.Packets.Incoming.Navigator
{
    class GoToHotelViewEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;


            if (Session.GetHabbo().InRoom)
            {
                Room OldRoom;

                if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                /*if(OldRoom.QueueingUsers.Contains(Session.GetHabbo()))
                {
                    OldRoom.QueueingUsers.Remove(OldRoom.QueueingUsers.First());
                    foreach (Habbo user in OldRoom.QueueingUsers)
                    {
                        user.GetClient().SendMessage(new RoomQueueComposer(OldRoom.QueueingUsers.IndexOf(user)));
                    }
                }*/

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, true, false);
            }
        }
    }
}
