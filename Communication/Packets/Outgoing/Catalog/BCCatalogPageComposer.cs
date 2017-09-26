﻿using System;
using System.Linq;

using Quasar.Core;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Catalog;
using Quasar.HabboHotel.Items.Utilities;
using Quasar.HabboHotel.Catalog.Utilities;
using System.Collections.Generic;
using Quasar.HabboHotel.GameClients;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    public class BCCatalogPageComposer : ServerPacket
    {
        public BCCatalogPageComposer(BCCatalogPage Page, string CataMode)
            : base(ServerPacketHeader.CatalogPageMessageComposer)
        {
            base.WriteInteger(Page.Id);
            base.WriteString(CataMode);
            base.WriteString(Page.Template);

            base.WriteInteger(Page.PageStrings1.Count);
            foreach (string s in Page.PageStrings1)
            {
                base.WriteString(s);
            }

            base.WriteInteger(Page.PageStrings2.Count);
            foreach (string s in Page.PageStrings2)
            {
                base.WriteString(s);
            }

            if (!Page.Template.Equals("frontpage") && !Page.Template.Equals("club_buy"))
            {

                base.WriteInteger(Page.Items.Count);
                foreach (var Item in Page.Items.Values)
                {
                    base.WriteInteger(Item.Id);
                    base.WriteString(Item.Name);
                    base.WriteBoolean(false);
                    base.WriteInteger(Item.CostCredits);

                    if (Item.CostDiamonds > 0)
                    {
                        base.WriteInteger(Item.CostDiamonds);
                        base.WriteInteger(5);
                    }
                    else if (Item.CostGOTWPoints > 0)
                    {
                        base.WriteInteger(Item.CostGOTWPoints);
                        base.WriteInteger(103);
                    }
                    else
                    {
                        base.WriteInteger(Item.CostPixels);
                        base.WriteInteger(0);
                    }
                    base.WriteBoolean(false);
                    if (Item.Data.InteractionType == InteractionType.DEAL)
                    {
                        foreach (CatalogDeal Deal in Page.Deals.Values)
                        {
                            base.WriteInteger(Deal.ItemDataList.Count);

                            foreach (CatalogItem DealItem in Deal.ItemDataList.ToList())
                            {
                                base.WriteString(DealItem.Data.Type.ToString());
                                base.WriteInteger(DealItem.Data.SpriteId);
                                base.WriteString("");
                                base.WriteInteger(DealItem.Amount);
                                base.WriteBoolean(false);
                            }
                            base.WriteInteger(0);
                            base.WriteBoolean(false);
                            base.WriteBoolean(true);
                            base.WriteString("");
                        }
                    }
                    else
                    {
                        if (Item.PredesignedId > 0)
                        {
                            base.WriteInteger(Page.PredesignedItems.Items.Count);
                            foreach (var predesigned in Page.PredesignedItems.Items.ToList())
                            {
                                ItemData Data = null;
                                if (QuasarEnvironment.GetGame().GetItemManager().GetItem(predesigned.Key, out Data)) { }
                                base.WriteString(Data.Type.ToString());
                                base.WriteInteger(Data.SpriteId);
                                base.WriteString(string.Empty);
                                base.WriteInteger(predesigned.Value);
                                base.WriteBoolean(false);
                            }

                            base.WriteInteger(0);
                            base.WriteBoolean(false);
                            base.WriteBoolean(true);
                            base.WriteString("");
                        }
                        else
                        {
                            base.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);

                            if (!string.IsNullOrEmpty(Item.Badge))
                            {
                                base.WriteString("b");
                                base.WriteString(Item.Badge);
                            }

                            base.WriteString(Item.Data.Type.ToString());
                            if (Item.Data.Type.ToString().ToLower() == "b")
                            {
                                base.WriteString(Item.Data.ItemName);
                            }
                            else
                            {
                                base.WriteInteger(Item.Data.SpriteId);
                                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                                {
                                    base.WriteString(Item.Name.Split('_')[2]);
                                }
                                else if (Item.Data.InteractionType == InteractionType.BOT)//Bots
                                {
                                    CatalogBot CatalogBot = null;
                                    if (!QuasarEnvironment.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot))
                                        base.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                                    else
                                        base.WriteString(CatalogBot.Figure);
                                }
                                else if (Item.ExtraData != null)
                                {
                                    base.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                                }
                                base.WriteInteger(Item.Amount);
                                base.WriteBoolean(Item.IsLimited);
                                if (Item.IsLimited)
                                {
                                    base.WriteInteger(Item.LimitedEditionStack);
                                    base.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                                }
                            }
                            base.WriteInteger(0);
                            base.WriteBoolean(false);

                            base.WriteBoolean(true);
                            base.WriteString("");
                        }

                    }
                }

                //}
                /*}*/
            }
            else
                base.WriteInteger(0);

            base.WriteInteger(-1);
            base.WriteBoolean(false);

            if (Page.Template == "frontpage4")
            {
                ICollection<Frontpage> FrontPage = QuasarEnvironment.GetGame().GetCatalogFrontPageManager().GetBCCatalogFrontPage();
                base.WriteInteger(FrontPage.Count);

                foreach (Frontpage front in FrontPage.ToList<Frontpage>())
                {
                    base.WriteInteger(front.Id());
                    base.WriteString(front.FrontName());
                    base.WriteString(front.FrontImage());
                    base.WriteInteger(0);
                    base.WriteString(front.FrontLink());
                    base.WriteInteger(-1);

                }
            }
        }
    }
}
