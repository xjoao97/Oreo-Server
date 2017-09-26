using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms.AI;
using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing.Rooms.AI.Pets;

namespace Quasar.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class GetPetTrainingPanelEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            int PetId = Packet.PopInt();

            RoomUser Pet = null;
            if (!Session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out Pet))
            {
                RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(PetId);
                if (User == null)
                    return;

                if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    return;

                Session.SendWhisper("Talvez um dia, boo boo");
                return;
            }

            if (Pet.RoomId != Session.GetHabbo().CurrentRoomId || Pet.PetData == null)
                return;

            Session.SendMessage(new PetTrainingPanelComposer(Pet.PetData.PetId, Pet.PetData.Level));
        }
    }
}
