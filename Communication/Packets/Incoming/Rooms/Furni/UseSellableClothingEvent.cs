using System;

using Quasar.Database.Interfaces;

using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Catalog.Clothing;

using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Inventory.AvatarEffects;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class UseSellableClothingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int ItemId = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.Data == null)
                return;

            if (Item.UserID != Session.GetHabbo().Id)
                return;

            if (Item.Data.InteractionType != InteractionType.PURCHASABLE_CLOTHING)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("changelooksucess", "Houve algum erro! Tente novamente e por favor avise a equipe.", ""));
                Console.WriteLine("Houve um erro com alguma Roupa na Loja.");
                return;
            }

            if (Item.Data.ClothingId == 0)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("changelooksucess", "Houve algum erro! Tente novamente e por favor avise a equipe.", ""));
                Console.WriteLine("Houve um erro com alguma Roupa na Loja.");
                return;
            }

            ClothingItem Clothing = null;
            if (!QuasarEnvironment.GetGame().GetCatalog().GetClothingManager().TryGetClothing(Item.Data.ClothingId, out Clothing))
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("changelooksucess", "Houve algum erro! Tente novamente e por favor avise a equipe.", ""));
                Console.WriteLine("Houve um erro com alguma Roupa na Loja.");
                return;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", Item.Id);
                dbClient.RunQuery();
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);

            Session.GetHabbo().GetClothing().AddClothing(Clothing.ClothingName, Clothing.PartIds);
            Session.SendMessage(new FigureSetIdsComposer(Session.GetHabbo().GetClothing().GetClothingAllParts));
            Session.SendMessage(new RoomNotificationComposer("figureset.redeemed.success"));
        }
    }
}