using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Items;

namespace Quasar.Communication.Packets.Outgoing.Rooms.FloorPlan
{
    class FloorPlanFloorMapComposer : ServerPacket
    {
        public FloorPlanFloorMapComposer(List<Point> Items)
            : base(ServerPacketHeader.FloorPlanFloorMapMessageComposer)
        {
            base.WriteInteger(Items.Count);
            foreach (Point Item in Items)
            {
                base.WriteInteger(Item.X);
                base.WriteInteger(Item.Y);
            }
        }
    }