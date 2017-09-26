﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Settings
{
    class RoomSettingsSavedComposer : ServerPacket
    {
        public RoomSettingsSavedComposer(int roomID)
            : base(ServerPacketHeader.RoomSettingsSavedMessageComposer)
        {
            base.WriteInteger(roomID);
        }
    }
}