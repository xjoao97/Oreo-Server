﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Catalog.Utilities;

namespace Quasar.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListComposer : ServerPacket
    {
        public FurniListComposer(List<Item> Items, ICollection<Item> Walls)
            : base(ServerPacketHeader.FurniListMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(1);

            base.WriteInteger(Items.Count + Walls.Count);
            foreach (Item Item in Items)
            {
                WriteItem(Item);
            }

            foreach (Item Item in Walls)
            {
                WriteItem(Item);
            }
        }

        public FurniListComposer(List<Item> Items, int page, int pages)
            : base(ServerPacketHeader.FurniListMessageComposer)
        {
            base.WriteInteger(pages);
            base.WriteInteger(page);

            base.WriteInteger(Items.Count);
            foreach (Item Item in Items)
            {
                WriteItem(Item);
            }
        }

        public FurniListComposer()
           : base(ServerPacketHeader.FurniListMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(1);

            base.WriteInteger(0);
        }



        private void WriteItem(Item Item)
        {
            base.WriteInteger(Item.Id);
            base.WriteString(Item.GetBaseItem().Type.ToString().ToUpper());
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);

            if (Item.LimitedNo > 0)
            {
                base.WriteInteger(1);
                base.WriteInteger(256);
                base.WriteString(Item.ExtraData);
                base.WriteInteger(Item.LimitedNo);
                base.WriteInteger(Item.LimitedTot);
            }

            else
                ItemBehaviourUtility.GenerateExtradata(Item, this);

            base.WriteBoolean(Item.GetBaseItem().AllowEcotronRecycle);
            base.WriteBoolean(Item.GetBaseItem().AllowTrade);
            base.WriteBoolean(Item.LimitedNo == 0 ? Item.GetBaseItem().AllowInventoryStack : false);
            base.WriteBoolean(ItemUtility.IsRare(Item));
            base.WriteInteger(-1);//Seconds to expiration.
            base.WriteBoolean(true);
            base.WriteInteger(-1);//Item RoomId

            if (!Item.IsWallItem)
            {
                base.WriteString(string.Empty);
                base.WriteInteger(0);
            }
        }
    }
}
