
using Emulator.Communication.Packets.Outgoing.Navigator;

using Emulator.Database.Interfaces;
using Emulator.HabboHotel.GameClients;
using Emulator.HabboHotel.Rooms;
using Emulator.HabboHotel.Users;
using Emulator.Communication.Packets.Incoming;
using Emulator.HabboHotel.Navigator;
using System.Data;
using Emulator.Communication.Packets.Outgoing.Rooms.Settings;

namespace Emulator.Communication.Packets.Incoming.Navigator
{
    public class ToggleStaffPickEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            //Check Current Sessions Rank...
            if (Session.GetHabbo().Rank < HabboStaticGameSettings.CanStaffPickRank)
                return;

            int RoomId = Packet.PopInt(); // obv.
            bool Add = Packet.PopBoolean(); //Adding or Deleting??!?

            //Let's load the RoomData for this Room.

            if (!HabboEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out Room Room))
                return;

            //hmm, cmon?
            if (Room == null)
                return;
            if (Session == null)
                return;
           

            //Let's check if it's already added, if so it must be a delete?!?
            if (!Add)
            {
                Room.DeleteStaffPick(Room.Id);              
                Room.RoomData.staffPick = false;
                Room.SendMessage(new RoomSettingsSavedComposer(Room.RoomId));
                Room.SendMessage(new RoomInfoUpdatedComposer(Room.RoomId));
                Session.SendWhisper("Susccess, you have deleted this room from the staff picks. Please 'update navigator to take effect.");
                return;
            }


            StaffPicks t = new StaffPicks(Session.GetHabbo().Id, Room.Name, Room.Description);
            HabboEnvironment.GetGame().GetNavigator().AddStaffPick(RoomId, t);
            Session.SendWhisper("Success, Room has been added to the staff picks. Please 'update navigator' to take effect.");

            //Insert the required Queries.
            Room.SetStaffPick(Room.Id);
            Room.InsertStaffPick(Room.Id, Room.RoomData.Name, Room.RoomData.Description, Session.GetHabbo().Username);

            //Update the room.
            Room.RoomData.staffPick = true;
            Room.SendMessage(new RoomSettingsSavedComposer(Room.RoomId));
            Room.SendMessage(new RoomInfoUpdatedComposer(Room.RoomId));
        }
    }
}