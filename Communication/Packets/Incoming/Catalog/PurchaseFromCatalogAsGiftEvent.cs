using Galaxy.Communication.Packets.Outgoing.Catalog;
using Galaxy.Communication.Packets.Outgoing.Inventory.Furni;
using Galaxy.Communication.Packets.Outgoing.Inventory.Purse;
using Galaxy.Communication.Packets.Outgoing.Moderation;
using Galaxy.Database.Interfaces;
using Galaxy.HabboHotel.Catalog;
using Galaxy.HabboHotel.Catalog.Utilities;
using Galaxy.HabboHotel.GameClients;
using Galaxy.HabboHotel.Items;
using Galaxy.HabboHotel.Items.Utilities;
using Galaxy.HabboHotel.Quests;
using Galaxy.HabboHotel.Users;
using Galaxy.Utilities;
using System;

namespace Galaxy.Communication.Packets.Incoming.Catalog
{
    public class PurchaseFromCatalogAsGiftEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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

            if (GalaxyServer.GetGame().GetSettingsManager().TryGetValue("room.item.gifts.enabled") != "1")
            {
                Session.SendNotification("Os gerentes do Hotel desabilitaram presentear!");
                return;
            }

            CatalogPage Page = null;
            if (!GalaxyServer.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                return;

            if ( !Page.Enabled || !Page.Visible || Page.MinimumRank  >  Session.GetHabbo().Rank || (Page.MinimumVIP > Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 1))
                return;

            CatalogItem Item = null;
            if (!Page.Items.TryGetValue(ItemId, out Item))
            {
                if (Page.ItemOffers.ContainsKey(ItemId))
                {
                    Item = Page.ItemOffers[ItemId];
                    if (Item == null)
                        return;
                }
                else
                    return;
            }

            if (!ItemUtility.CanGiftItem(Item))
                return;

            ItemData PresentData = null;
            if (!GalaxyServer.GetGame().GetItemManager().GetGift(SpriteId, out PresentData) || PresentData.InteractionType != InteractionType.GIFT)
                return;

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= Item.CostCredits;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= Item.CostPixels;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
            }

            if (Item.CostGotw > 0)
            {
                Session.GetHabbo().GOTWPoints -= Item.CostGotw;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
            }


            Habbo Habbo = GalaxyServer.GetHabboByUsername(GiftUser);
            if (Habbo == null)
            {
                Session.SendMessage(new GiftWrappingErrorComposer());
                return;
            }

            if (!Habbo.AllowGifts)
            {
                Session.SendNotification("Ops! Esse usu??rio n??o aceita presentes!");
                return;
            }

            if ((DateTime.Now - Session.GetHabbo().LastGiftPurchaseTime).TotalSeconds <= 5.0)
            {
                Session.SendNotification("Voc?? est?? comprando presentes muito r??pido! Aguarde 5 segundos!");
        
                Session.GetHabbo().GiftPurchasingWarnings += 1;
                if (Session.GetHabbo().GiftPurchasingWarnings >= 25)
                    Session.GetHabbo().SessionGiftBlocked = true;
                return;
            }

            if (Session.GetHabbo().SessionGiftBlocked)
                return;


            string ED = GiftUser + Convert.ToChar(5) + GiftMessage + Convert.ToChar(5) + Session.GetHabbo().Id + Convert.ToChar(5) + Item.Data.Id + Convert.ToChar(5) + SpriteId + Convert.ToChar(5) + Ribbon + Convert.ToChar(5) + Colour;

            int NewItemId = 0;
            using (IQueryAdapter dbClient = GalaxyServer.GetDatabaseManager().GetQueryReactor())
            {
                //Insert the dummy item.
                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (@baseId, @habboId, @extra_data)");
                dbClient.AddParameter("baseId", PresentData.Id);
                dbClient.AddParameter("habboId", Habbo.Id);
                dbClient.AddParameter("extra_data", ED);
                NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                string ItemExtraData = null;
                switch (Item.Data.InteractionType)
                {
                    case InteractionType.NONE:
                        ItemExtraData = "";
                        break;

                    #region Pet handling

                    case InteractionType.PET:

                        try
                        {
                            string[] Bits = Data.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race); // to trigger any possible errors

                            if (PetUtility.CheckPetName(PetName))
                                return;

                            if (Race.Length > 2)
                                return;

                            if (Color.Length != 6)
                                return;

                            GalaxyServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
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
                                Number = Double.Parse(Data, GalaxyServer.CultureInfo);
                        }
                        catch
                        {

                        }

                        ItemExtraData = Number.ToString().Replace(',', '.');
                        break; // maintain extra data // todo: validate

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
                        ItemExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Manequim Padr??o";
                        break;

                    case InteractionType.BADGE_DISPLAY:
                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                        {
                            Session.SendMessage(new BroadcastMessageAlertComposer("Ei, parece que voc?? j?? tem esse emblema!"));
                            return;
                        }

                        ItemExtraData = Data + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        break;

                    default:
                        ItemExtraData = Data;
                        break;
                }

                //Insert the present, forever.
                dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES (@itemId, @baseId, @extra_data)");
                dbClient.AddParameter("itemId", NewItemId);
                dbClient.AddParameter("baseId", Item.Data.Id);
                dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ItemExtraData) ? "" : ItemExtraData));
                dbClient.RunQuery();

                //Here we're clearing up a record, this is dumb, but okay.
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @deleteId LIMIT 1");
                dbClient.AddParameter("deleteId", NewItemId);
                dbClient.RunQuery();
            }


            Item GiveItem = ItemFactory.CreateGiftItem(PresentData, Habbo, ED, ED, NewItemId, 0, 0);
            if (GiveItem != null)
            {
                GameClient Receiver = GalaxyServer.GetGame().GetClientManager().GetClientByUserID(Habbo.Id);
                if (Receiver != null)
                {
                    Receiver.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    Receiver.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Receiver.SendMessage(new PurchaseOKComposer());
                    Receiver.SendMessage(new FurniListAddComposer(GiveItem));
                    Receiver.SendMessage(new FurniListUpdateComposer());
                }

                if (Habbo.Id != Session.GetHabbo().Id && !string.IsNullOrWhiteSpace(GiftMessage))
                {
                    GalaxyServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GiftGiver", 1);
                    if (Receiver != null)
                        GalaxyServer.GetGame().GetAchievementManager().ProgressAchievement(Receiver, "ACH_GiftReceiver", 1);
                    GalaxyServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.GIFT_OTHERS);
                }
            }
       
            Session.SendMessage(new PurchaseOKComposer(Item, PresentData, Item.Items));

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= Item.CostCredits;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostPixels > 0)
            {
                Session.GetHabbo().Duckets -= Item.CostPixels;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));//Love you, Tom.
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().Diamonds -= Item.CostDiamonds;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
            }

            if (Item.CostGotw > 0)
            {
                Session.GetHabbo().GOTWPoints -= Item.CostGotw;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
            }

            Session.GetHabbo().LastGiftPurchaseTime = DateTime.Now;
        }
    }
}