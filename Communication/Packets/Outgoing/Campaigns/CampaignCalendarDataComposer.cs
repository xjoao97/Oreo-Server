using Quasar.HabboHotel.Calendar;
using System;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Campaigns
{
    class CampaignCalendarDataComposer : ServerPacket
    {
        public CampaignCalendarDataComposer(bool[] OpenedBoxes)
            : base(ServerPacketHeader.CampaignCalendarDataMessageComposer)
        {
            base.WriteString(QuasarEnvironment.GetGame().GetCalendarManager().GetCampaignName()); //Nome da Campanha
            base.WriteString("asd"); //Não tem função na SWF
            base.WriteInteger(QuasarEnvironment.GetGame().GetCalendarManager().GetUnlockDays()); //Dia Atual
            base.WriteInteger(QuasarEnvironment.GetGame().GetCalendarManager().GetTotalDays()); //Dias
            int OpenedCount = 0;
            int LateCount = 0;

            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                if (OpenedBoxes[i])
                {
                    OpenedCount++;
                }
                else
                {
                    //Dias Atual (EVITAMOS)
                    if (QuasarEnvironment.GetGame().GetCalendarManager().GetUnlockDays() == i)
                        continue;

                    LateCount++;
                }
            }
            //Caixas abertas até o momento
            base.WriteInteger(OpenedCount);
            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                if (OpenedBoxes[i])
                {
                    base.WriteInteger(i);
                }
            }

            //Total de Caixas (Custom: Não altera nada aqui, deixa como está eu vou revisar)
            base.WriteInteger(LateCount);
            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                //Dias Atual (EVITAMOS)
                if (QuasarEnvironment.GetGame().GetCalendarManager().GetUnlockDays() == i)
                    continue;

                if (!OpenedBoxes[i])
                    base.WriteInteger(i);
            }
        }
    }
}
