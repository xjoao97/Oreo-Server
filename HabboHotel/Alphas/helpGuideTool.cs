/*﻿using HabboEvents;
using Atlanta.HabboHotel.GameClients;
using Atlanta.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quasar.Communication.Packets.Outgoing;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Alphas;

namespace Quasar.Protocol.Messages.outgoing
{
    class helpGuideTool : ServerPacket
    {
        public helpGuideTool(bool onDuty)
            : base(ServerPacketHeader.helpGuideTool)
           {
            var countGuides = 0;
            var countAlphas = 0;
            var countGuardian = 0;

            foreach (GameClient c in AlphaManager.getInstance().onService)
            {
                if (c.GetHabbo() == null) continue;
                if (c.GetHabbo().rankHelper == HabboHotel.Users.typeOfHelper.Guardian)
                    countGuardian++;
                else if (c.GetHabbo().rankHelper == HabboHotel.Users.typeOfHelper.Alpha)
                    countAlphas++;
                else if (c.GetHabbo().rankHelper == HabboHotel.Users.typeOfHelper.Guide)
                    countGuides++;
            }
            var m = new ServerMessage(Outgoing.helpGuideTool);
            m.WriteBool(onDuty);
            m.WriteInt(countGuides);
            m.WriteInt(countAlphas);
            m.WriteInt(countGuardian);
            return m;
        }
    }
}*/
