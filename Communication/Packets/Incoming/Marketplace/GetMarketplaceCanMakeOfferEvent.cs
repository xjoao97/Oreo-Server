﻿using Quasar.Communication.Packets.Outgoing.Marketplace;

namespace Quasar.Communication.Packets.Incoming.Marketplace
{
    class GetMarketplaceCanMakeOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ErrorCode = (Session.GetHabbo().TradingLockExpiry > 0 ? 6 : 1);

            Session.SendMessage(new MarketplaceCanMakeOfferResultComposer(ErrorCode));
        }
    }
}