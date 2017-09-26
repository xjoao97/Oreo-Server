using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Catalog;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.BuildersClub;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    class GetCatalogModeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string PageMode = Packet.PopString();

            if (PageMode == "NORMAL")
                Session.SendMessage(new CatalogIndexComposer(Session, QuasarEnvironment.GetGame().GetCatalog().GetPages(), PageMode));//, Sub));
            else if (PageMode == "BUILDERS_CLUB")
                Session.SendMessage(new CatalogIndexComposer(Session, QuasarEnvironment.GetGame().GetCatalog().GetBCPages(), PageMode));
        }
    }
}
