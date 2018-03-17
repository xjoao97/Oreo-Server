using Oreo.HabboHotel.GameClients;
using Oreo.Communication.Packets.Outgoing.Catalog;
using Oreo.Communication.Packets.Outgoing.BuildersClub;

namespace Oreo.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            Session.SendMessage(new CatalogIndexComposer(Session, OreoServer.GetGame().GetCatalog().GetPages(Session, -1)));
            Session.SendMessage(new CatalogItemDiscountComposer());
            Session.SendMessage(new BCBorrowedItemsComposer());
        }
    }
}