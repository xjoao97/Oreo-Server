﻿using System;
using Quasar.Communication.Packets.Incoming;

using Quasar.Utilities;
using Quasar.HabboHotel.Users;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Groups;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Users.Inventory;

using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Items.Utilities;
using Quasar.HabboHotel.Quests;
using Quasar.HabboHotel.Catalog.Utilities;
using Quasar.Communication.Packets.Outgoing.Moderation;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogAsGiftEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string Data = Packet.PopString();
            string GiftUser = StringCharFilter.Escape(Packet.PopString());
            string GiftMessage = StringCharFilter.Escape(Packet.PopString().Replace(Convert.ToChar(5), ' '));
            int SpriteId = Packet.PopInt();
            int Ribbon = Packet.PopInt();
            int Colour = Packet.PopInt();
            bool dnow = Packet.PopBoolean();

            if (QuasarEnvironment.GetDBConfig().DBData["gifts_enabled"] != "1")
            {
                Session.SendNotification("O envio de presentes foi desativado temporariamente!");
                return;
            }

            CatalogPage Page = null;
            if (!QuasarEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;
            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk)
                return;
            if ( !Page.Enabled || !Page.Visible || Page.MinimumRank > Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                return;

            CatalogItem Item = null;
            if (!Page.Items.TryGetValue(ItemId, out Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = (CatalogItem)Page.ItemOffers[ItemId];
                    if (Item == null)
                        return;
                }
                else
                    return;
            }

            if (!ItemUtility.CanGiftItem(Item))
                return;

            ItemData PresentData = null;
            if (!QuasarEnvironment.GetGame().GetItemManager().GetGift(SpriteId, out PresentData) || PresentData.InteractionType != InteractionType.GIFT)
                return;

            if (Session.GetHabbo().Credits < Item.CostCredits)
            {
                Session.SendMessage(new PresentDeliverErrorMessageComposer(true, false));
                return;
            }

            if (Session.GetHabbo().Duckets < Item.CostPixels)
            {
                Session.SendMessage(new PresentDeliverErrorMessageComposer(false, true));
                return;
            }

            Habbo Habbo = QuasarEnvironment.GetHabboByUsername(GiftUser);
            if (Habbo == null)
            {
                Session.SendMessage(new GiftWrappingErrorComposer());
                return;
            }

            if (!Habbo.AllowGifts)
            {
                Session.SendNotification("Oops, esse usuário não aceita presentes!");
                return;
            }

            if (Session.GetHabbo().Rank < 4)
            {
                if ((DateTime.Now - Session.GetHabbo().LastGiftPurchaseTime).TotalSeconds <= 5.0)
                {
                    Session.SendNotification("Vá com calma, você está enviando presentes muito rápido!");

                    Session.GetHabbo().GiftPurchasingWarnings += 1;
                    if (Session.GetHabbo().GiftPurchasingWarnings >= 25)
                        Session.GetHabbo().SessionGiftBlocked = true;
                        return;
                }
            }


            if (Session.GetHabbo().SessionGiftBlocked)
                return;

            string ED = GiftUser + Convert.ToChar(5) + GiftMessage + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + SpriteId + Convert.ToChar(5) + Ribbon + Convert.ToChar(5) + Colour;

            int NewItemId = 0;
            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {

                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES ('" + PresentData.Id + "', '" + Habbo.Id + "', @extra_data)");
                dbClient.AddParameter("extra_data", ED);
                NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                string ItemExtraData = null;
                switch (Item.Data.InteractionType)
                {
                    case InteractionType.NONE:
                        ItemExtraData = "";
                        break;

                    #region Pet handling

                    case InteractionType.pet0:
                    case InteractionType.pet1:
                    case InteractionType.pet2:
                    case InteractionType.pet3:
                    case InteractionType.pet4:
                    case InteractionType.pet5:
                    case InteractionType.pet6:
                    case InteractionType.pet7:
                    case InteractionType.pet8:
                    case InteractionType.pet9:
                    case InteractionType.pet10:
                    case InteractionType.pet11:
                    case InteractionType.pet12:
                    case InteractionType.pet13:
                    case InteractionType.pet14:
                    case InteractionType.pet15:
                    case InteractionType.pet16:
                    case InteractionType.pet17:
                    case InteractionType.pet18:
                    case InteractionType.pet19:
                    case InteractionType.pet20:
                    case InteractionType.pet21:
                    case InteractionType.pet22:
                    case InteractionType.pet28:
                    case InteractionType.pet29:
                    case InteractionType.pet30:

                        try
                        {
                            string[] Bits = Data.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race); //Para desencadear possíveis erros

                            if (PetUtility.CheckPetName(PetName))
                                return;

                            if (Race.Length > 2)
                                return;

                            if (Color.Length != 6)
                                return;

                            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                        }
                        catch
                        {
                            return;
                        }

                        break;

                    #endregion

                    case InteractionType.FLOOR:
                    case InteractionType.WALLPAPER:
                    case InteractionType.LANDSCAPE:

                        Double Number = 0;
                        try
                        {
                            if (string.IsNullOrEmpty(Data))
                                Number = 0;
                            else
                                Number = Double.Parse(Data, QuasarEnvironment.CultureInfo);
                        }
                        catch
                        {

                        }

                        ItemExtraData = Number.ToString().Replace(',', '.');
                        break;

                    case InteractionType.POSTIT:
                        ItemExtraData = "FFFF33";
                        break;

                    case InteractionType.MOODLIGHT:
                        ItemExtraData = "1,1,1,#000000,255";
                        break;

                    case InteractionType.TROPHY:
                        ItemExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + Data;
                        break;

                    case InteractionType.MANNEQUIN:
                        ItemExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Manequim Padrão";
                        break;

                    case InteractionType.BADGE_DISPLAY:
                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                        {
                            Session.SendMessage(new BroadcastMessageAlertComposer("Oops, aparentemente você não possui este emblema!"));
                            return;
                        }

                        ItemExtraData = Data + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        break;

                    default:
                        ItemExtraData = Data;
                        break;
                }

                dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES ('" + NewItemId + "', '" + Item.Data.Id + "', @extra_data)");
                dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ItemExtraData) ? "" : ItemExtraData));
                dbClient.RunQuery();

                dbClient.RunQuery("DELETE FROM `items` WHERE `id` = " + NewItemId + " LIMIT 1;");
            }

            Item GiveItem = ItemFactory.CreateGiftItem(PresentData, Habbo, ED, ED, NewItemId, 0, 0);
            if (GiveItem != null)
            {
                GameClient Receiver = QuasarEnvironment.GetGame().GetClientManager().GetClientByUserID(Habbo.Id);
                if (Receiver != null)
                {
                    if (Receiver.GetHabbo().Rank <= 5)
                    {

                     Receiver.SendNotification("Você recebeu um presente de " + Session.GetHabbo().Username + " abra seu inventário!");

                    }
                        {

                        Receiver.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                        Receiver.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                        Receiver.SendMessage(new PurchaseOKComposer());
                        Receiver.SendMessage(new FurniListAddComposer(GiveItem));
                        Receiver.SendMessage(new FurniListUpdateComposer());

                    }
                }
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GiftGiver", 1);
                    QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Receiver, "ACH_GiftReceiver", 1);
                    QuasarEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.GIFT_OTHERS);
                    Session.SendNotification("Presente enviado!");

            }

            Session.SendMessage(new PurchaseOKComposer(Item, PresentData));

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= Item.CostCredits;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= Item.CostPixels;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
            }

            Session.GetHabbo().LastGiftPurchaseTime = DateTime.Now;
        }
    }
}
