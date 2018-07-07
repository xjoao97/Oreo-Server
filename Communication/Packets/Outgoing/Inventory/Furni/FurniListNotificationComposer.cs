using Plus.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Plus.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListNotificationComposer : ServerPacket
    {
        public FurniListNotificationComposer(List<Item> items, int Type)
            : base(ServerPacketHeader.FurniListNotificationMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(Type);
            base.WriteInteger(items.Count);
            foreach(Item i in items)
            {
                base.WriteInteger(i.Id);
            }
        }
 
        public FurniListNotificationComposer(int Id, int Type)
            : base(ServerPacketHeader.FurniListNotificationMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(Type);
            base.WriteInteger(1);
            base.WriteInteger(Id);
        }
 
    }
}