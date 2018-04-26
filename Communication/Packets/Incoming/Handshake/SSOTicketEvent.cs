using System;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Handshake;

public class SSOTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null /*|| Session.RC4Client == null*/ || Session.GetHabbo() != null)
                return;

            string SSO = Packet.PopString();

            if (!string.IsNullOrEmpty(SSO))
                Session.TryAuthenticate(SSO);
        }
    }
