using Quasar.HabboHotel.Pathfinding;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Rooms.Avatar
{
    class LookToEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = null;
            if (!QuasarEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (User.IsAsleep)
                return;
            
            User.UnIdle();

            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if ((X == User.X && Y == User.Y) || User.IsWalking || User.RidingHorse)
                return;

            int Rot = Rotation.Calculate(User.X, User.Y, X, Y);

            User.SetRot(Rot, false);
            User.UpdateNeeded = true;

            if (User.RidingHorse)
            {
                RoomUser Horse = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByVirtualId(User.HorseID);
                if (Horse != null)
                {
                    Horse.SetRot(Rot, false);
                    Horse.UpdateNeeded = true;
                }
            }

        }
    }
}
