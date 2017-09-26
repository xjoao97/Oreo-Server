﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.GameClients;

using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Database.Interfaces;

namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
{
    class PickupObjectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;
            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk || QuasarStaticGameSettings.IsGoingToBeClose)
                return;
            int Unknown = Packet.PopInt();
            int ItemId = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Room.ForSale)
            {
                Session.SendWhisper("No se puede editar la Sala mientras está a la venta.");
                Session.SendWhisper("Cancela la Venta de la Sala expulsando a todos escribe ':unload' (sin las '')");
                return;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.POSTIT)
                return;

            Boolean ItemRights = false;
            if (Item.UserID == Session.GetHabbo().Id || Room.CheckRights(Session, false))
                ItemRights = true;
            else if (Room.Group != null && Room.CheckRights(Session, false, true))//Room has a group, this user has group rights.
                ItemRights = true;
            else if (Session.GetHabbo().GetPermissions().HasRight("room_item_take"))
                ItemRights = true;

            if (ItemRights == true)
            {
                if (Item.GetBaseItem().InteractionType == InteractionType.TENT || Item.GetBaseItem().InteractionType == InteractionType.TENT_SMALL)
                    Room.RemoveTent(Item.Id, Item);

                if (Item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `room_items_moodlight` WHERE `item_id` = '" + Item.Id + "' LIMIT 1");
                    }
                }
                else if (Item.GetBaseItem().InteractionType == InteractionType.TONER)
                {
                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `room_items_toner` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                }


                if (Item.UserID == Session.GetHabbo().Id)
                {
                    Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                }
                else if (Session.GetHabbo().GetPermissions().HasRight("room_item_take"))
                {
                    Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                    Session.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                    Session.GetHabbo().GetInventoryComponent().UpdateItems(false);

                }
                else//Item is being ejected.
                {
                    GameClient targetClient = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(Item.UserID);
                    if (targetClient != null && targetClient.GetHabbo() != null)
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(targetClient, Item.Id);
                        targetClient.GetHabbo().GetInventoryComponent().AddNewItem(Item.Id, Item.BaseItem, Item.ExtraData, Item.GroupId, true, true, Item.LimitedNo, Item.LimitedTot);
                        targetClient.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    else//No, query time.
                    {
                        Room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                        using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                        }
                    }
                }

                QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PICK);
            }
        }
    }
}
