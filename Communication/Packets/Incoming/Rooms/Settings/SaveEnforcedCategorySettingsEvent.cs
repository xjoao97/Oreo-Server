using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Navigator;
using Quasar.Database.Interfaces;

namespace Quasar.Communication.Packets.Incoming.Rooms.Settings
{
    class SaveEnforcedCategorySettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Packet.PopInt(), out Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            int CategoryId = Packet.PopInt();
            int TradeSettings = Packet.PopInt();

            if (TradeSettings < 0 || TradeSettings > 2)
                TradeSettings = 0;

            SearchResultList SearchResultList = null;
            if (!QuasarEnvironment.GetGame().GetNavigator().TryGetSearchResultList(CategoryId, out SearchResultList))
            {
                CategoryId = 36;
            }

            if (SearchResultList.CategoryType != NavigatorCategoryType.CATEGORY || SearchResultList.RequiredRank > Session.GetHabbo().Rank)
            {
                CategoryId = 36;
            }
        }
    }
}
