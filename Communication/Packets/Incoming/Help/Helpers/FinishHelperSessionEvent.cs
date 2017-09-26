using Quasar.Communication.Packets.Outgoing.Help.Helpers;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Help.Helpers
{
    class FinishHelperSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Voted = Packet.PopBoolean();
            var Element = HelperToolsManager.GetElement(Session);
            if (Element is HelperCase)
            {
                if (Voted)
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Session.GetHabbo().Username + ", obrigado por colaborar com o projeto!.", "catalog/open/socioxd"));
                else
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("ambassador", "" + Session.GetHabbo().Username + ", obrigado por colaborar com o projeto!.", "catalog/open/socioxd"));
            }

            Element.Close();
        }
    }
}
