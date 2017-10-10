using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new MarketplaceConfigurationComposer());
        }
    }
}