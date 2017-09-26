﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;

namespace Quasar.Communication.Packets.Incoming.Rooms.Action
{
    class KickUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (!Room.CheckRights(Session) && Room.WhoCanKick != 2 && Room.Group == null)
                return;

            if (Room.Group != null && !Room.CheckRights(Session, false, true))
                return;

            int UserId = Packet.PopInt();
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
            if (User == null || User.IsBot)
                return;

            if (Room.CheckRights(User.GetClient(), true) || User.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            Room.GetRoomUserManager().RemoveUserFromRoom(User.GetClient(), true, true);
            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModKickSeen", 1);
        }
    }
}
