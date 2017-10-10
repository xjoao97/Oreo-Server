namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    public class MarketplaceConfigurationComposer : ServerPacket
    {
        public MarketplaceConfigurationComposer()
            : base(ServerPacketHeader.MarketplaceConfigurationMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(1);
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(25000);
            base.WriteInteger(48);
            base.WriteInteger(7);
            //ByWulles
        }
    }
}