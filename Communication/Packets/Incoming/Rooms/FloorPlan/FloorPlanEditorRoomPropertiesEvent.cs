using System.Linq;
using System.Collections.Generic;

using Oreo.HabboHotel.Rooms;
using Oreo.HabboHotel.Items;
using Oreo.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Oreo.Communication.Packets.Outgoing.Rooms.Engine;
using System.Drawing;

namespace Oreo.Communication.Packets.Incoming.Rooms.FloorPlan
{
    class FloorPlanEditorRoomPropertiesEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;
           
            if (Room.GetGameMap().Model == null)
                return;

            List<Point> Squares = new List<Point>();
            Room.GetRoomItemManager().GetFloor.ToList().ForEach(Item =>
            {
                Item.GetCoords.ForEach(Point =>
                {
                    if (!Squares.Contains(Point))
                        Squares.Add(Point);
                });
            });

            Session.SendMessage(new FloorPlanFloorMapComposer(Squares));
            Session.SendMessage(new FloorPlanSendDoorComposer(Room.GetGameMap().Model.DoorX, Room.GetGameMap().Model.DoorY, Room.GetGameMap().Model.DoorOrientation));
            Session.SendMessage(new RoomVisualizationSettingsComposer(Room.WallThickness, Room.FloorThickness, OreoServer.EnumToBool(Room.Hidewall.ToString())));

            Squares.Clear();
            Squares = null;
        }
    }
}
