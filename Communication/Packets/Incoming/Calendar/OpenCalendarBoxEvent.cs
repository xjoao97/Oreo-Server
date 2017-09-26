using Quasar.Communication.Packets.Outgoing.Campaigns;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.Communication.Packets.Outgoing.Users;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Calendar
{
    class OpenCalendarBoxEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string CampaignName = Packet.PopString();
            int CampaignDay = Packet.PopInt();

            if (CampaignName != QuasarEnvironment.GetGame().GetCalendarManager().GetCampaignName())
                return;

            if (CampaignDay < 0 || CampaignDay > QuasarEnvironment.GetGame().GetCalendarManager().GetTotalDays() - 1 || CampaignDay < QuasarEnvironment.GetGame().GetCalendarManager().GetUnlockDays())
                return;

            if (CampaignDay > QuasarEnvironment.GetGame().GetCalendarManager().GetUnlockDays())
                return;

            if (Session.GetHabbo().calendarGift[CampaignDay])
                return;


            Session.GetHabbo().calendarGift[CampaignDay] = true;

            //Atualizar pacote
            Session.SendMessage(new CalendarPrizesComposer(QuasarEnvironment.GetGame().GetCalendarManager().GetCampaignDay(CampaignDay + 1)));
            Session.SendMessage(new CampaignCalendarDataComposer(Session.GetHabbo().calendarGift));


            string Gift = QuasarEnvironment.GetGame().GetCalendarManager().GetGiftByDay(CampaignDay + 1);
            string GiftType = Gift.Split(':')[0];
            string GiftValue = Gift.Split(':')[1];

            switch (GiftType.ToLower())
            {
                case "itemid":
                    {
                        ItemData Item = null;
                        if (!QuasarEnvironment.GetGame().GetItemManager().GetItem(int.Parse(GiftValue), out Item))
                        {
                            return;
                        }

                        Item GiveItem = ItemFactory.CreateSingleItemNullable(Item, Session.GetHabbo(), "", "");
                        if (GiveItem != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);

                            Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
                            Session.SendMessage(new FurniListUpdateComposer());
                        }

                        Session.GetHabbo().GetInventoryComponent().UpdateItems(false);
                    }
                    break;

                case "badge":
                    {
                        Session.GetHabbo().GetBadgeComponent().GiveBadge(GiftValue, true, Session);
                    }
                    break;

                case "diamonds":
                    {
                        Session.GetHabbo().Diamonds += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, 0, 5));
                    }
                    break;

                case "gotwpoints":
                    {
                        Session.GetHabbo().GOTWPoints += int.Parse(GiftValue);
                        Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, 0, 103));
                    }
                    break;

                case "vip":
                    {
                        var IsVIP = Session.GetHabbo().GetClubManager().HasSubscription("club_vip");
                        if (IsVIP)
                        {
                            Session.SendMessage(new AlertNotificationHCMessageComposer(4));
                        }
                        else
                        {
                            Session.SendMessage(new AlertNotificationHCMessageComposer(5));
                        }
                        if (Session.GetHabbo().Rank > 2)
                        {
                            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                            }
                        }
                        else
                        {
                            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunQuery("UPDATE `users` SET `rank` = '2' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                                dbClient.RunQuery("UPDATE `users` SET `rank_vip` = '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                            }
                        }

                        Session.GetHabbo().GetClubManager().AddOrExtendSubscription("club_vip", int.Parse(GiftValue) * 24 * 3600, Session);
                        Session.GetHabbo().GetBadgeComponent().GiveBadge("VIP", true, Session);

                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_VipClub", 1);
                        Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
                    }
                    break;
            }

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("INSERT INTO user_campaign_gifts VALUES (NULL, '" + Session.GetHabbo().Id + "','" + CampaignName + "','" + (CampaignDay + 1) + "')");
            }
        }
    }
}
