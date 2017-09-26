﻿using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.GameCenter
{
    class LoadGameComposer : ServerPacket
    {
        public LoadGameComposer(GameData GameData, string SSOTicket)
            : base(ServerPacketHeader.LoadGameMessageComposer)
        {
           base.WriteInteger(GameData.GameId);
           base.WriteString("1365260055982");
           base.WriteString(GameData.ResourcePath + GameData.GameSWF);
           base.WriteString("best");
           base.WriteString("showAll");
           base.WriteInteger(60);
           base.WriteInteger(10);
           base.WriteInteger(8);
           base.WriteInteger(6);
           base.WriteString("assetUrl");
           base.WriteString(GameData.ResourcePath + GameData.GameAssets);
           base.WriteString("habboHost");
           base.WriteString("http://fuseus-private-httpd-fe-1");
           base.WriteString("accessToken");
           base.WriteString(SSOTicket);
           base.WriteString("gameServerHost");
           base.WriteString(GameData.GameServerHost);
           base.WriteString("gameServerPort");
           base.WriteString(GameData.GameServerPort);
           base.WriteString("socketPolicyPort");
           base.WriteString(GameData.GameServerHost);
        }
    }
}
