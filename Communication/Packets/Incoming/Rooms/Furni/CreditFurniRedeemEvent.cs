using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Emulator.HabboHotel.Rooms;
using Emulator.HabboHotel.Items;
using Emulator.Communication.Packets.Outgoing.Inventory.Purse;
using Emulator.Communication.Packets.Outgoing.Inventory.Furni;

using Emulator.Database.Interfaces;


namespace Emulator.Communication.Packets.Incoming.Rooms.Furni
{
    class CreditFurniRedeemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            if (!HabboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            if (HabboEnvironment.GetDBConfig().DBData["exchange_enabled"] != "1")
            {
                Session.SendNotification("The hotel managers have temporarilly disabled exchanging!");
                return;
            }

            Item Exchange = Room.GetRoomItemHandler().GetItem(Packet.PopInt());
            if (Exchange == null)
                return;

            if (Exchange.Data.InteractionType != InteractionType.EXCHANGE)
                return;


            int Value = Exchange.Data.BehaviourData;

            if (Exchange.Data.ItemName.StartsWith("POINTS_"))
            {
               
                    using (IQueryAdapter dbClient = HabboEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @exchangeId LIMIT 1");
                        dbClient.AddParameter("exchangeId", Exchange.Id);
                        dbClient.RunQuery();
                    }
                    Session.SendWhisper("You have successfully redeemed " + Value + " point(s)!");
                

                if (Value > 0)
                {
                    Session.GetHabbo().GOTWPoints = Session.GetHabbo().GOTWPoints + Value;
                    Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, Value, 103));
                }

                Session.SendMessage(new FurniListUpdateComposer());
                Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id, false);
                Session.GetHabbo().GetInventoryComponent().RemoveItem(Exchange.Id);     
            }
            else if (Exchange.Data.ItemName.StartsWith("DIAMONDS_"))
            {
                if (Value > 0)
                {
                    Session.GetHabbo().Diamonds += Value;
                    Session.SendMessage(new ActivityPointsComposer(Session.GetHabbo().Diamonds, Session.GetHabbo().Diamonds, 5));
                }

                using (IQueryAdapter dbClient = HabboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @exchangeId LIMIT 1");
                    dbClient.AddParameter("exchangeId", Exchange.Id);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new FurniListUpdateComposer());
                Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id, false);
                Session.GetHabbo().GetInventoryComponent().RemoveItem(Exchange.Id);
            }
            else
            {
                if (Value > 0)
                {
                    Session.GetHabbo().Credits += Value;
                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                }

                using (IQueryAdapter dbClient = HabboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @exchangeId LIMIT 1");
                    dbClient.AddParameter("exchangeId", Exchange.Id);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new FurniListUpdateComposer());
                Room.GetRoomItemHandler().RemoveFurniture(null, Exchange.Id, false);
                Session.GetHabbo().GetInventoryComponent().RemoveItem(Exchange.Id);
            }
        }
    }
}