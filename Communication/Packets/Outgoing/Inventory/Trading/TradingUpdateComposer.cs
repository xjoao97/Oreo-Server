using System.Linq;
using Quasar.HabboHotel.Rooms.Trading;
using Quasar.HabboHotel.Items;


namespace Quasar.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingUpdateComposer : ServerPacket
    {
        public TradingUpdateComposer(Trade Trade)
            : base(ServerPacketHeader.TradingUpdateMessageComposer)
        {
            if (Trade.Users.Count() < 2)
                return;
            
            foreach(TradeUser user in Trade.Users)
            {
                base.WriteInteger(user.GetClient().GetHabbo().Id);
                base.WriteInteger(user.OfferedItems.Count);

                SerializeUserItems(user);

                base.WriteInteger(user.OfferedItems.Count);
                base.WriteInteger(0);

            }
            

        }
        private void SerializeUserItems(TradeUser User)
        {
            //base.WriteInteger(User.OfferedItems.Count);//While
            foreach (Item Item in User.OfferedItems)
            {
                base.WriteInteger(Item.Id);
                base.WriteString(Item.Data.Type.ToString().ToUpper());
                base.WriteInteger(Item.Id);
                base.WriteInteger(Item.Data.SpriteId);
                base.WriteInteger(1);
                base.WriteBoolean(true);

                //Func called _SafeStr_15990
                base.WriteInteger(0);
                base.WriteString("");

                //end Func called
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteInteger(0);
                if (Item.Data.Type.ToString().ToUpper() == "S")
                    base.WriteInteger(0);
            }

        }
    }
}