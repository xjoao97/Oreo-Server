using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    public class WiredRewardAlertComposer : ServerPacket
    {
        public WiredRewardAlertComposer(int codeMsg)
            : base(ServerPacketHeader.WiredRewardAlertComposer)
        {
            base.WriteInteger(codeMsg);
        }
    }
}

/*
*   6 = Prêmio recebido
*   7 = Cartão recebido
*   0 = Desculpe, Os prêmios disponíveis são limitados entregues!
*   1 = Você já ganhou este prêmio, cada usuário só pode ganhar o mesmo prêmio uma vez!
*   2 = VocÊ já foi premiado hoje, tente novamente amanhã
*   3 = Você já recebeu o presente a uma hora, tente daqui a pouco!
*   4 = Oops, você não teve sorte dessa vez, não desista teente novamente mais tarde!
*   5 = Você já coletou todos os prêmios hoje!
*   8 = Você já recebeu um prêmio, tente novamente em um minuto!
 */
