/*using HabboEvents;
using Atlanta.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlanta.Protocol.Messages.outgoing
{
    class onGuideSessionError
    {
        internal static ServerMessage composer(int error)
        {

             * 0 - Padrão - Na sessão de guia destacada
             * 1 - Sem Helpers - Nenhum ajudante disponível para passeios
             * 2 - Sem guias - Sem guias disponíveis para pedido de bullying

            var m = new ServerMessage(Outgoing.onGuideSessionError);
            m.WriteInt(error);
            return m;
        }
    }
}*/
