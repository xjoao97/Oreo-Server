using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Catalog;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.Items;
using System;
using System.Data;

namespace Quasar.Communication.Packets.Incoming.Catalog
{
    class BuyTargettedOfferMessage : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)

        {
            #region RETURN VALUES
            var offer = QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer;
            var habbo = Session.GetHabbo();
            if (offer == null || habbo == null)
            {
                Session.SendMessage(new PurchaseErrorComposer(1));
                return;
            }
            #endregion

            #region FIELDS
            Packet.PopInt();
            var amount = Packet.PopInt();
            if (amount > offer.Limit)
            {
                Session.SendMessage(new PurchaseErrorComposer(1));
                return;
            }
            var creditsCost = int.Parse(offer.Price[0]) * amount;
            var extraMoneyCost = int.Parse(offer.Price[1]) * amount;
            #endregion

            //#region CREDITS COST
            //if (creditsCost > 0)
            //{
            //    if (habbo.Credits < creditsCost)
            //    {
            //        Session.SendMessage(new PurchaseErrorComposer(1));
            //        return;
            //    }

            //    habbo.Credits -= creditsCost;
            //    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits - creditsCost));
            //}
            //#endregion

            //#region EXTRA MONEY COST
            //if (extraMoneyCost > 0)
            //{
            //    #region GET MONEY TYPE AND DISCOUNT
            //    switch (offer.MoneyType)
            //    {
            //        #region DUCKETS COST
            //        case "duckets":
            //            {
            //                if (habbo.Duckets < extraMoneyCost)
            //                {
            //                    Session.SendMessage(new PurchaseErrorComposer(1));
            //                    return;
            //                }

            //                //habbo.Duckets -= extraMoneyCost;
            //                Session.GetHabbo().Duckets -= extraMoneyCost;
            //                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
            //                break;
            //            }
            //        #endregion

            //        #region DIAMONDS COST
            //        case "diamonds":
            //            {
            //                if (habbo.Diamonds < extraMoneyCost)
            //                {
            //                    Session.SendMessage(new PurchaseErrorComposer(1));
            //                    return;
            //                }

            //                //habbo.Diamonds -= extraMoneyCost;
            //                Session.GetHabbo().Diamonds -= extraMoneyCost;
            //                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
            //                break;
            //            }
            //            #endregion

            //            //#region OTHER COST
            //            //default:
            //            //    goto case "duckets";
            //            //    #endregion
            //    }
            //    #endregion

            //    //habbo.UpdateExtraMoneyBalance();
            //}
            //#endregion

            #region BUY AND CREATE ITEMS PROGRESS
            TargetedOffers TargetedOffer = QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer;
            using (IQueryAdapter dbQuery = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbQuery.SetQuery("SELECT targeted_buy FROM users where id = " + habbo.Id + " LIMIT 1");
                DataTable count = dbQuery.getTable();
                foreach (DataRow Row in count.Rows)
                {
                    int offer2 = Convert.ToInt32(Row["targeted_buy"]);


                    if (TargetedOffer.Limit == offer2)
                    {
                        Session.SendMessage(new RoomCustomizedAlertComposer("Você atingiu o limite de ofertas!"));
                    }

                    else

                    {
                        using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                            dbClient.runFastQuery("UPDATE users SET targeted_buy = targeted_buy +1 WHERE id = " + Session.GetHabbo().Id + ";");
                        foreach (var product in offer.Products)
                        {
                            #region CHECK PRODUCT TYPE
                            switch (product.ItemType)
                            {
                                #region NORMAL ITEMS CASE
                                case "item":
                                    {
                                        ItemData item = null;
                                        if (!QuasarEnvironment.GetGame().GetItemManager().GetItem(int.Parse(product.Item), out item)) return;
                                        if (item == null) return;
                                        var cItem = ItemFactory.CreateSingleItemNullable(item, Session.GetHabbo(), string.Empty, string.Empty);
                                        if (cItem != null)
                                        {
                                            Session.GetHabbo().GetInventoryComponent().TryAddItem(cItem);

                                            Session.SendMessage(new FurniListAddComposer(cItem));
                                            Session.SendMessage(new FurniListUpdateComposer());

                                        }

                                        Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                                        break;
                                    }
                                #endregion

                                #region BADGE CASE
                                case "badge":
                                    {
                                        if (habbo.GetBadgeComponent().HasBadge(product.Item))
                                        {
                                            //Session.SendMessage(new RoomCustomizedAlertComposer("Olhe para frente, você pode fugir!"));
                                            //break;
                                        }

                                        habbo.GetBadgeComponent().GiveBadge(product.Item, true, Session);
                                        break;
                                    }
                                    #endregion
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion

            #region CREDITS COST
            if (creditsCost > 0)
            {
                if (habbo.Credits < creditsCost)
                {
                    Session.SendMessage(new PurchaseErrorComposer(1));
                    return;
                }

                habbo.Credits -= creditsCost;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits - creditsCost));
            }
            #endregion

            #region EXTRA MONEY COST
            if (extraMoneyCost > 0)
            {
                #region GET MONEY TYPE AND DISCOUNT
                switch (offer.MoneyType)
                {
                    #region DUCKETS COST
                    case "duckets":
                        {
                            if (habbo.Duckets < extraMoneyCost)
                            {
                                Session.SendMessage(new PurchaseErrorComposer(1));
                                return;
                            }

                            //habbo.Duckets -= extraMoneyCost;
                            Session.GetHabbo().Duckets -= extraMoneyCost;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
                            break;
                        }
                    #endregion

                    #region DIAMONDS COST
                    case "diamonds":
                        {
                            if (habbo.Diamonds < extraMoneyCost)
                            {
                                Session.SendMessage(new PurchaseErrorComposer(1));
                                return;
                            }

                            //habbo.Diamonds -= extraMoneyCost;
                            Session.GetHabbo().Diamonds -= extraMoneyCost;
                            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                            break;
                        }
                        #endregion

                        //#region OTHER COST
                        //default:
                        //    goto case "duckets";
                        //    #endregion
                }
                #endregion

                //habbo.UpdateExtraMoneyBalance();
            }
            #endregion

            #region RE-OPEN TARGETED BOX
                    TargetedOffers TargetedOffer2 = QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer;

                            int offer22 = Session.GetHabbo()._TargetedBuy;


                            if (TargetedOffer2.Limit > offer22)
                            {
                                Session.SendMessage(QuasarEnvironment.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                            }
                        }
                    }
            #endregion
        }
