using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    class GetClubOffersMessageComposer : ServerPacket
    {
        public GetClubOffersMessageComposer()
            : base(ServerPacketHeader.GetClubOffersMessageComposer)
        {
            int credits = 1;
            int diamonds = 3;

            base.WriteInteger(0);//Não sei
            base.WriteString("asd");//Não sei
            base.WriteBoolean(true);//Não sei
            base.WriteInteger(75);//Resultado créditos
            base.WriteInteger(5);//Resultado extra
            base.WriteInteger(-1);//Não sei
            base.WriteBoolean(true);//Prolongar
            base.WriteInteger(1);//Preço multiplicado
            base.WriteInteger(1);//Não sei
            base.WriteBoolean(true);//Ativar moeda extra

            base.WriteInteger(0);//Não sei
            base.WriteInteger(0);//Não sei
            base.WriteInteger(0);//Não sei
            base.WriteInteger(0);//Não sei
            base.WriteInteger(80);//Créditos
            base.WriteInteger(5);//Extra
            base.WriteInteger(105);//Tipo de Moeda
            base.WriteInteger(1);//Dias

        }
    }
}
