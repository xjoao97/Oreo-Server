using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Games;
using Quasar.HabboHotel.Users;

namespace Quasar.Communication.Packets.Outgoing.GameCenter
{
    public class Game2WeeklyLeaderboardComposer : ServerPacket
    {
        public Game2WeeklyLeaderboardComposer(GameData GameData, ICollection<Habbo> Habbos)
            : base(ServerPacketHeader.Game2WeeklyLeaderboardMessageComposer)
        {
            base.WriteInteger(2014);
            base.WriteInteger(41);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(1581);

            int num = 0;

            base.WriteInteger(Habbos.Count);
            foreach (Habbo Habbo in Habbos.ToList())
            {
                num++;
                base.WriteInteger(Habbo.Id);
                base.WriteInteger(Habbo.FastfoodScore);
                base.WriteInteger(num);
                base.WriteString(Habbo.Username);
                base.WriteString(Habbo.Look);
                base.WriteString(Habbo.Gender.ToLower());
            }

            base.WriteInteger(0);
            base.WriteInteger(GameData.GameId);
        }
    }
}
