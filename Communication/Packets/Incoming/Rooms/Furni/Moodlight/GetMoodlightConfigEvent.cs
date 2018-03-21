using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Oreo.HabboHotel.Rooms;
using Oreo.HabboHotel.Items;
using Oreo.HabboHotel.Items.Data.Moodlight;

using Oreo.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;

namespace Oreo.Communication.Packets.Incoming.Rooms.Furni.Moodlight
{
    class GetMoodlightConfigEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;


            if (!OreoServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
                return;

            if (!Room.CheckRights(Session, true))
                return;

            if (Room.MoodlightData == null)
            {
                foreach (Item item in Room.GetRoomItemHandler().GetWall.ToList())
                {
                    if (item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                        Room.MoodlightData = new MoodlightData(item.Id);
                }
            }

            if (Room.MoodlightData == null)
                return;

            Session.SendMessage(new MoodlightConfigComposer(Room.MoodlightData));
        }
    }
}
