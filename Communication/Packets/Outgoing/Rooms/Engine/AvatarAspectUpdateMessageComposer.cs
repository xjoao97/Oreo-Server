﻿using System;
using System.Linq;
using System.Text;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Engine
{
    class AvatarAspectUpdateMessageComposer : ServerPacket
    {
        public AvatarAspectUpdateMessageComposer(string Figure, string Gender)
            : base(ServerPacketHeader.AvatarAspectUpdateMessageComposer)
        {
            base.WriteString(Figure);
            base.WriteString(Gender);

        }
    }
}