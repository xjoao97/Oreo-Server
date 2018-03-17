using System.Collections.Generic;
using Oreo;
using Oreo.Communication.Packets.Outgoing;
using Oreo.HabboHotel.Catalog;
using Oreo.HabboHotel.GameClients;


public class CatalogIndexComposer : ServerPacket
{
    public CatalogIndexComposer(GameClient session, ICollection<CatalogPage> pages)
        :base(ServerPacketHeader.CatalogIndexMessageComposer)
    {
        base.WriteBoolean(true);
        base.WriteInteger(0);
        base.WriteInteger(-1);
        base.WriteString("root");
        base.WriteString("");
        base.WriteInteger(0);

        base.WriteInteger(pages.Count);
        foreach (CatalogPage page in pages)
        {
            Append(session, page);
        }

        base.WriteBoolean(false);
        base.WriteString("NORMAL");
    }

    public void Append(GameClient session, CatalogPage page)
    {
        ICollection<CatalogPage> pages = OreoServer.GetGame().GetCatalog().GetPages(session, page.Id);

        base.WriteBoolean(page.Visible);
        base.WriteInteger(page.Icon);
        base.WriteInteger(page.Enabled ? page.Id : -1);
        base.WriteString(page.PageLink);
        base.WriteString(page.Caption);

        base.WriteInteger(page.ItemOffers.Count);
        foreach (int key in page.ItemOffers.Keys)
        {
            base.WriteInteger(key);
        }

        base.WriteInteger(pages.Count);
        foreach (CatalogPage nextPage in pages)
        {
            Append(session, nextPage);
        }
    }
}