using System;
using System.Linq;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Users;

using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Items.Data.Moodlight;
using Quasar.HabboHotel.Items.Data.Toner;

namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
{
    class PlaceObjectEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;
            if (Session.GetHabbo().Rank > 3 && !Session.GetHabbo().StaffOk || QuasarStaticGameSettings.IsGoingToBeClose)
                return;
            int ItemId = 0;
            string[] Data = null;

            string RawData = Packet.PopString();
            Data = RawData.Split(' ');

            if (!int.TryParse(Data[0], out ItemId))
                return;

            bool HasRights = false;
            if (Room.CheckRights(Session, false, true))
                HasRights = true;

            if (!HasRights)
            {
                Session.SendMessage(new RoomNotificationComposer("furni_placement_error", "message", "${room.error.cant_set_not_owner}"));
                return;
            }

            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
            if (Item == null)
                return;

            if (Room.ForSale)
            {
                Session.SendWhisper("No se puede editar la Sala mientras está a la venta.");
                Session.SendWhisper("Cancela la Venta de la Sala expulsando a todos escribe ':unload' (sin las '')");
                return;
            }

            if (Room.GetRoomItemHandler().GetWallAndFloor.Count() > QuasarStaticGameSettings.RoomFurnitureLimit)
            {
                Session.SendNotification("não pode ter mais de " + QuasarStaticGameSettings.RoomFurnitureLimit + " items no quarto!");
                return;
            }
            else if (Item.GetBaseItem().ItemName.ToLower().Contains("cf") && Room.OwnerId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("room_item_place_exchange_anywhere"))
            {
                Session.SendNotification("Você não pode colocar moedas regeneráveis no quarto!");
                return;
            }

            switch (Item.GetBaseItem().InteractionType)
            {
                #region Interaction Types
                case InteractionType.MOODLIGHT:
                    {
                        MoodlightData moodData = Room.MoodlightData;
                        if (moodData != null && Room.GetRoomItemHandler().GetItem(moodData.ItemId) != null)
                        {
                            Session.SendNotification("Você só pode ter um regulador por quarto!");
                            return;
                        }
                        break;
                    }
                case InteractionType.TONER:
                    {
                        TonerData tonerData = Room.TonerData;
                        if (tonerData != null && Room.GetRoomItemHandler().GetItem(tonerData.ItemId) != null)
                        {
                            Session.SendNotification("Você só pode colocar um fundo de quarto!");
                            return;
                        }
                        break;
                    }
                case InteractionType.HOPPER:
                    {
                        if (Room.GetRoomItemHandler().HopperCount > 0)
                        {
                            Session.SendNotification("Você só pode colocar um por quarto!");
                            return;
                        }
                        break;
                    }
                    
                    case InteractionType.FOOTBALL:
                    {
                       if (Room.CountFootBall(Room.Id) >= 4)
                       {
                            Session.SendNotification("Você só pode ter 4 bolas por quarto.");
                            return;

                      }
                        break;

                    }

                case InteractionType.TENT:
                case InteractionType.TENT_SMALL:
                    {
                        Room.AddTent(Item.Id);
                        break;
                    }
                    #endregion
            }

            if (!Item.IsWallItem)
            {
                if (Data.Length < 4)
                    return;

                int X = 0;
                int Y = 0;
                int Rotation = 0;

                if (!int.TryParse(Data[1], out X)) { return; }
                if (!int.TryParse(Data[2], out Y)) { return; }
                if (!int.TryParse(Data[3], out Rotation)) { return; }

                Item RoomItem = new Item(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, X, Y, 0, Rotation, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, string.Empty, Room);
                if (Room.GetRoomItemHandler().SetFloorItem(Session, RoomItem, X, Y, Rotation, true, false, true))
                {
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);

                    if (Session.GetHabbo().Id == Room.OwnerId)
                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFurniCount", 1, false);

                    if (RoomItem.IsWired)
                    {
                        try { Room.GetWired().LoadWiredBox(RoomItem); }
                        catch { Console.WriteLine(Item.GetBaseItem().InteractionType); }
                    }
                }
                else
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
                    return;
                }
            }
            else if (Item.IsWallItem)
            {
                string[] CorrectedData = new string[Data.Length - 1];

                for (int i = 1; i < Data.Length; i++)
                {
                    CorrectedData[i - 1] = Data[i];
                }

                string WallPos = string.Empty;

                if (TrySetWallItem(Session.GetHabbo(), Item, CorrectedData, out WallPos))
                {
                    try
                    {
                        Item RoomItem = new Item(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, 0, 0, 0, 0, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, WallPos, Room);

                        if (Room.GetRoomItemHandler().SetWallItem(Session, RoomItem))
                        {
                            Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
                            if (Session.GetHabbo().Id == Room.OwnerId)
                                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFurniCount", 1, false);
                        }
                    }
                    catch
                    {
                        Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
                        return;
                    }
                }
                else
                {
                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
                    return;
                }
            }
        }

        private static bool TrySetWallItem(Habbo Habbo, Item item, string[] data, out string position)
        {
            if (data.Length != 3 || !data[0].StartsWith(":w=") || !data[1].StartsWith("l=") || (data[2] != "r" && data[2] != "l"))
            {
                position = null;
                return false;
            }

            string wBit = data[0].Substring(3, data[0].Length - 3);
            string lBit = data[1].Substring(2, data[1].Length - 2);

            if (!wBit.Contains(",") || !lBit.Contains(","))
            {
                position = null;
                return false;
            }

            int w1 = 0;
            int w2 = 0;
            int l1 = 0;
            int l2 = 0;

            int.TryParse(wBit.Split(',')[0], out w1);
            int.TryParse(wBit.Split(',')[1], out w2);
            int.TryParse(lBit.Split(',')[0], out l1);
            int.TryParse(lBit.Split(',')[1], out l2);

            /*if (!Habbo.HasFuse("super_admin") && (w1 < 0 || w2 < 0 || l1 < 0 || l2 < 0 || w1 > 200 || w2 > 200 || l1 > 200 || l2 > 200))
            {
                position = null;
                return false;
            }*/



            string WallPos = ":w=" + w1 + "," + w2 + " l=" + l1 + "," + l2 + " " + data[2];

            position = WallPositionCheck(WallPos);

            return (position != null);
        }

        public static string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                {
                    return null;
                }
                if (wallPosition.Contains(Convert.ToChar(9)))
                {
                    return null;
                }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                    return null;

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                if (widthX < -1000 || widthY < -1 || widthX > 700 || widthY > 700)
                    return null;

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                if (lengthX < -1 || lengthY < -1000 || lengthX > 700 || lengthY > 700)
                    return null;

                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return null;
            }
        }
    }
}

//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;

//using Quasar.HabboHotel.Rooms;
//using Quasar.HabboHotel.Items;
//using Quasar.HabboHotel.Users;

//using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
//using Quasar.HabboHotel.Items.Data;
//using Quasar.HabboHotel.Items.Data.Moodlight;
//using Quasar.HabboHotel.Items.Data.Toner;

//namespace Quasar.Communication.Packets.Incoming.Rooms.Engine
//{
//    class PlaceObjectEvent : IPacketEvent
//    {
//        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
//        {
//            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
//                return;
//            if (Session.GetHabbo().Rank > 2 && !Session.GetHabbo().StaffOk)
//                return;
//            Room Room = null;
//            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
//                return;

//            int ItemId = 0;
//            string[] Data = null;

//            string RawData = Packet.PopString();
//            Data = RawData.Split(' ');

//            if (!int.TryParse(Data[0], out ItemId))
//                return;

//            if (Data.Length < 4)
//                return;

//            bool HasRights = false;
//            if (Room.CheckRights(Session, false, true))
//                HasRights = true;

//            if (Room.Type == "private")
//            {
//                int X = 0;
//                int Y = 0;
//                int Rotation = 0;

//                if (!int.TryParse(Data[1], out X)) { return; }
//                if (!int.TryParse(Data[2], out Y)) { return; }
//                if (!int.TryParse(Data[3], out Rotation)) { return; }

//                if (Room.GetGameMap().IsRentableSpace(X, Y, Session))
//                {
//                    Item Items = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);

//                    Item RoomItem = new Item(Items.Id, Room.RoomId, Items.BaseItem, Items.ExtraData, X, Y, 0, Rotation, Session.GetHabbo().Id, Items.GroupId, Items.LimitedNo, Items.LimitedTot, string.Empty, Room);

//                    if (Room.GetRoomItemHandler().SetFloorItem(Session, RoomItem, X, Y, Rotation, true, false, true))
//                    {
//                        Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);

//                        if (Session.GetHabbo().Id == Room.OwnerId)
//                            QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFurniCount", 1, false);

//                        if (RoomItem.IsWired)
//                        {
//                            try { Room.GetWired().LoadWiredBox(RoomItem); }
//                            catch { Console.WriteLine(Items.GetBaseItem().InteractionType); }
//                        }
//                    }
//                    else
//                    {
//                        Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
//                        return;
//                    }
//                }
//                else if (!Room.GetGameMap().IsRentableSpace(X, Y, Session) && Session.GetHabbo().Id != Room.OwnerId && !HasRights)
//                {
//                    Session.SendWhisper("Isso não é seu");
//                    return;
//                }

//                else

//                if (!HasRights)
//                {
//                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
//                    return;
//                }
//            }



//            Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
//            if (Item == null)
//                return;

//            if (Room.GetRoomItemHandler().GetWallAndFloor.Count() > QuasarStaticGameSettings.RoomFurnitureLimit)
//            {
//                Session.SendNotification("no se puede tener mas de " + QuasarStaticGameSettings.RoomFurnitureLimit + " furnis en una sala!");
//                return;
//            }
//            else if (Item.GetBaseItem().ItemName.ToLower().Contains("cf") && Room.OwnerId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("room_item_place_exchange_anywhere"))
//            {
//                Session.SendNotification("No se puede colocar monedas canjeables en esta sala!");
//                return;
//            }


//            switch (Item.GetBaseItem().InteractionType)
//            {
//                #region Interaction Types
//                case InteractionType.MOODLIGHT:
//                    {
//                        MoodlightData moodData = Room.MoodlightData;
//                        if (moodData != null && Room.GetRoomItemHandler().GetItem(moodData.ItemId) != null)
//                        {
//                            Session.SendNotification("Solo puedes tener un (1) regulador por sala!");
//                            return;
//                        }
//                        break;
//                    }
//                case InteractionType.TONER:
//                    {
//                        TonerData tonerData = Room.TonerData;
//                        if (tonerData != null && Room.GetRoomItemHandler().GetItem(tonerData.ItemId) != null)
//                        {
//                            Session.SendNotification("Solo puedes tener un (1) pinta fondo por sala!");
//                            return;
//                        }
//                        break;
//                    }
//                case InteractionType.HOPPER:
//                    {
//                        if (Room.GetRoomItemHandler().HopperCount > 0)
//                        {
//                            Session.SendNotification("Solo puedes tener un (1) SaltaSalas en esta habitacion!");
//                            return;
//                        }
//                        break;
//                    }

//                case InteractionType.TENT:
//                case InteractionType.TENT_SMALL:
//                    {
//                        Room.AddTent(Item.Id);
//                        break;
//                    }
//                    #endregion
//            }

//            if (!Item.IsWallItem)
//            {
//                if (Data.Length < 4)
//                    return;

//                int X = 0;
//                int Y = 0;
//                int Rotation = 0;

//                if (!int.TryParse(Data[1], out X)) { return; }
//                if (!int.TryParse(Data[2], out Y)) { return; }
//                if (!int.TryParse(Data[3], out Rotation)) { return; }

//                Item RoomItem = new Item(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, X, Y, 0, Rotation, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, string.Empty, Room);

//                /*if (Room.GetGameMap().GetRoomItemForSquare2(X, Y) && !Room.GetGameMap().HasStackTool(X, Y) && RoomItem.GetBaseItem().InteractionType != InteractionType.STACKTOOL)
//                {
//                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Lo siento, no puedes apilar este furni aquí", ""));
//                    return; // FIX ese cuadro ya tiene un furni.
//                }*/

//                if (Room.GetRoomItemHandler().SetFloorItem(Session, RoomItem, X, Y, Rotation, true, false, true))
//                {
//                    Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);

//                    if (Session.GetHabbo().Id == Room.OwnerId)
//                        QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFurniCount", 1, false);

//                    if (RoomItem.IsWired)
//                    {
//                        try { Room.GetWired().LoadWiredBox(RoomItem); }
//                        catch { }
//                    }
//                }
//                else
//                {
//                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
//                    return;
//                }
//            }
//            else if (Item.IsWallItem)
//            {
//                string[] CorrectedData = new string[Data.Length - 1];

//                for (int i = 1; i < Data.Length; i++)
//                {
//                    CorrectedData[i - 1] = Data[i];
//                }

//                string WallPos = string.Empty;

//                if (TrySetWallItem(Session.GetHabbo(), Item, CorrectedData, out WallPos))
//                {
//                    try
//                    {
//                        Item RoomItem = new Item(Item.Id, Room.RoomId, Item.BaseItem, Item.ExtraData, 0, 0, 0, 0, Session.GetHabbo().Id, Item.GroupId, Item.LimitedNo, Item.LimitedTot, WallPos, Room);

//                        if (Room.GetRoomItemHandler().SetWallItem(Session, RoomItem))
//                        {
//                            Session.GetHabbo().GetInventoryComponent().RemoveItem(ItemId);
//                            if (Session.GetHabbo().Id == Room.OwnerId)
//                                QuasarEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomDecoFurniCount", 1, false);
//                        }
//                    }
//                    catch
//                    {
//                        Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
//                        return;
//                    }
//                }
//                else
//                {
//                    Session.SendMessage(RoomNotificationComposer.SendBubble("furni_placement_error", "Você não tem permissão para isso!", ""));
//                    return;
//                }
//            }
//        }

//        private static bool TrySetWallItem(Habbo Habbo, Item item, string[] data, out string position)
//        {
//            if (data.Length != 3 || !data[0].StartsWith(":w=") || !data[1].StartsWith("l=") || (data[2] != "r" && data[2] != "l"))
//            {
//                position = null;
//                return false;
//            }

//            string wBit = data[0].Substring(3, data[0].Length - 3);
//            string lBit = data[1].Substring(2, data[1].Length - 2);

//            if (!wBit.Contains(",") || !lBit.Contains(","))
//            {
//                position = null;
//                return false;
//            }

//            int w1 = 0;
//            int w2 = 0;
//            int l1 = 0;
//            int l2 = 0;

//            int.TryParse(wBit.Split(',')[0], out w1);
//            int.TryParse(wBit.Split(',')[1], out w2);
//            int.TryParse(lBit.Split(',')[0], out l1);
//            int.TryParse(lBit.Split(',')[1], out l2);
//            //
//            //if (!Habbo.HasFuse("super_admin") && (w1 < 0 || w2 < 0 || l1 < 0 || l2 < 0 || w1 > 200 || w2 > 200 || l1 > 200 || l2 > 200))
//            //{
//            //    position = null;
//            //    return false;
//            //}



//            string WallPos = ":w=" + w1 + "," + w2 + " l=" + l1 + "," + l2 + " " + data[2];

//            position = WallPositionCheck(WallPos);

//            return (position != null);
//        }

//        public static string WallPositionCheck(string wallPosition)
//        {
//            //:w=3,2 l=9,63 l
//            try
//            {
//                if (wallPosition.Contains(Convert.ToChar(13)))
//                {
//                    return null;
//                }
//                if (wallPosition.Contains(Convert.ToChar(9)))
//                {
//                    return null;
//                }

//                string[] posD = wallPosition.Split(' ');
//                if (posD[2] != "l" && posD[2] != "r")
//                    return null;

//                string[] widD = posD[0].Substring(3).Split(',');
//                int widthX = int.Parse(widD[0]);
//                int widthY = int.Parse(widD[1]);
//                if (widthX < -1000 || widthY < -1 || widthX > 700 || widthY > 700)
//                    return null;

//                string[] lenD = posD[1].Substring(2).Split(',');
//                int lengthX = int.Parse(lenD[0]);
//                int lengthY = int.Parse(lenD[1]);
//                if (lengthX < -1 || lengthY < -1000 || lengthX > 700 || lengthY > 700)
//                    return null;

//                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
//            }
//            catch
//            {
//                return null;
//            }
//        }
//    }
//}
