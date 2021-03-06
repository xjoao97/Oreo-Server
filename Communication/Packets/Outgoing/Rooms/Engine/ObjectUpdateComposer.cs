using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.Utilities;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.Users;


namespace Quasar.Communication.Packets.Outgoing.Rooms.Engine
{
    class ObjectUpdateComposer : ServerPacket
    {
        public ObjectUpdateComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ObjectUpdateMessageComposer)
        {
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);
            base.WriteInteger(Item.GetX);
            base.WriteInteger(Item.GetY);
            base.WriteInteger(Item.Rotation);
            base.WriteString(String.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
            base.WriteString(String.Empty);

            if (Item.LimitedNo > 0)
            {
                base.WriteInteger(1);
                base.WriteInteger(256);
                base.WriteString(Item.ExtraData);
                base.WriteInteger(Item.LimitedNo);
                base.WriteInteger(Item.LimitedTot);
            }
            else if (Item.Data.InteractionType == InteractionType.INFO_TERMINAL)
            {
                base.WriteInteger(0);
                base.WriteInteger(1);
                base.WriteInteger(1);
                base.WriteString("internalLink");
                base.WriteString(Item.ExtraData);
            }
            else if (Item.Data.InteractionType == InteractionType.FX_PROVIDER)
            {
                base.WriteInteger(0);
                base.WriteInteger(1);
                base.WriteInteger(1);
                base.WriteString("effectId");
                base.WriteString(Item.ExtraData);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATA)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteString("6");
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteString((int.Parse(Item.ExtraData) == 1) ? "8" : "6");
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(1);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATATRIGGERED)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString((Item.ExtraData.Length <= 0) ? "0" : "2");
                if (Item.ExtraData.Length <= 0) base.WriteInteger(0);
                else base.WriteInteger(int.Parse(Item.ExtraData));
                base.WriteInteger(1);
            }

            else if (Item.Data.InteractionType == InteractionType.EASTEREGG)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(20);
            }

            else if (Item.Data.InteractionType == InteractionType.MAGICEGG)
            {
                base.WriteInteger(0);
                base.WriteInteger(7);
                base.WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
                    base.WriteInteger(0);
                }
                else
                {
                    base.WriteInteger(int.Parse(Item.ExtraData));
                }
                base.WriteInteger(23);
            }
            else
            {

                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

            base.WriteInteger(-1); // to-do: check
            base.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            base.WriteInteger(UserId);
        }
    }

    class UpdateFootBallComposer : ServerPacket
    {

        public UpdateFootBallComposer(Item Item, int newX, int newY, double newZ)
            : base(ServerPacketHeader.ObjectUpdateMessageComposer)
        {
            base.WriteInteger(Item.Id);
            base.WriteInteger(Item.GetBaseItem().SpriteId);
            base.WriteInteger(newX);
            base.WriteInteger(newY);
            base.WriteInteger(4); // rot;
            base.WriteString((String.Format("{0:0.00}", TextHandling.GetString(newZ))));
            base.WriteString((String.Format("{0:0.00}", TextHandling.GetString(newZ))));
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteString(Item.ExtraData);
            base.WriteInteger(-1);
            base.WriteInteger(0);
            base.WriteInteger(Item.UserID);
        }
    }
}
