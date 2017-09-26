using System;
using System.Linq;
using System.Text;

using Quasar.HabboHotel.Items.Televisions;
using Quasar.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class ToggleYouTubeVideoEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string VideoId = Packet.PopString();

            Session.SendMessage(new GetYouTubeVideoComposer(ItemId, VideoId));
        }
    }
}
