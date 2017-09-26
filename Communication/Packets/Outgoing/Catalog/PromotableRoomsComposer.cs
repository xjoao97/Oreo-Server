﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Quasar.HabboHotel.Rooms;
namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    class PromotableRoomsComposer : ServerPacket
    {
        public PromotableRoomsComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.PromotableRoomsMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(Rooms.Count);

            foreach (RoomData Data in Rooms)
            {
                base.WriteInteger(Data.Id);
               base.WriteString(Data.Name);
                base.WriteBoolean(false);
            }
        }
    }
}
