using System;

using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Rooms.Games;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms.Games.Teams;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Items.Interactor
{
    class InteractorPressurePad : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
                return;

            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) > 2)
                return;

            int count = int.Parse(Item.ExtraData);
            count++;
            Item.ExtraData = count + "";
            Item.UpdateState(true, true);
            Session.SendMessage(new RoomNotificationComposer("Alternador de cor."));
        }

        public void OnTrigger(GameClients.GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
                return;

            if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) > 2)
                return;

            int count = int.Parse(Item.ExtraData);
                count++;
                Item.ExtraData = count + "";
                Item.UpdateState(true, true);
                //Session.SendMessage(new RoomNotificationComposer("Piso mudou de cor!"));
        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}
