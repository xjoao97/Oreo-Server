using Quasar.Communication.Packets.Outgoing.Moderation;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Incoming.Rooms.Action
{
    class AmbassadorWarningMessageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            int UserId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            int Time = Packet.PopInt();

            Room Room = Session.GetHabbo().CurrentRoom;
            RoomUser Target = Room.GetRoomUserManager().GetRoomUserByHabbo(QuasarEnvironment.GetUsernameById(UserId));
            if (Target == null)
                return;

            long nowTime = QuasarEnvironment.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("abuse", "Espere pelo menos 1 minuto para voltar a enviar um alerta.", ""));
                return;
            }

            else
            QuasarEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("advice", "" + Session.GetHabbo().Username + " enviou uma lerta de embaixador para você " + Target.GetClient().GetHabbo().Username + ", clique para ir até lá.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            Target.GetClient().SendMessage(new BroadcastMessageAlertComposer("<b><font size='15px' color='#c40101'>Mensagem dos embaixadores<br></font></b>Os embaixadores consideram sua atitude negativa, revise seu comportamento para que um moderador não possa tomar uma atitude!"));

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;
        }
    }
}
