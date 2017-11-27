using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    class ClubGiftsComposer : ServerPacket
    {
        public ClubGiftsComposer()
            : base(ServerPacketHeader.ClubGiftsMessageComposer)
        {
            base.WriteInteger(231);
            //base.WriteInteger(0);
            base.WriteInteger(1);

            base.WriteInteger(1);
            /*
            this._-1L8 = _arg1._-1u3();
            this._-1iD = _arg1.readString();
            this._-2zc = _arg1.readBoolean();
            this._-6i = _arg1._-1u3();
            this._-4-j = _arg1._-1u3();
            this._-3h3 = _arg1._-1u3();
            this._-0jj = _arg1.readBoolean();
            var k:int = _arg1._-1u3();
            this._-0zK = new <_-0YF>[];
            var k:int;
            while (k < k)
           {
               this._-0zK.push(new _-0YF(_arg1));
               k++;
           }
           this._-rM = _arg1._-1u3();
           this._-5dA = _arg1.readBoolean();
           this._-0jf = _arg1.readB
           this._-5L5 = _arg1.readString();
            */

            base.WriteInteger(202); //Id da Oferta
            base.WriteString("Oferta"); //Nome
            base.WriteBoolean(false); //Rentável ou Compra
            base.WriteInteger(0); //Valor em Moedas
            base.WriteInteger(0); //Valor em Duckets
            base.WriteInteger(0); //Valor em Gotw
            base.WriteBoolean(false);

            base.WriteInteger(1);
            {
                base.WriteString("b");
                base.WriteString("ADM");
                //base.WriteInteger(8228);
                //base.WriteString("");
                //base.WriteInteger(1);
                //base.WriteBoolean(true);
            }
            base.WriteInteger(0);
            base.WriteBoolean(false);
            base.WriteBoolean(true);
            base.WriteString("");


            base.WriteInteger(1);

            base.WriteInteger(202);


            base.WriteBoolean(true);
            base.WriteInteger(256);
            base.WriteBoolean(true);
            base.WriteInteger(1);
            base.WriteBoolean(true);


            //{
            //    base.WriteInteger(12701);
            //    base.WriteString("hc16_1");
            //    base.WriteBoolean(false);
            //    base.WriteInteger(1);
            //    base.WriteInteger(0);
            //    base.WriteInteger(0);
            //    base.WriteBoolean(true);
            //    base.WriteInteger(1);
            //    {
            //        base.WriteString("s");
            //        base.WriteInteger(8228);
            //        base.WriteString("");
            //        base.WriteInteger(1);
            //        base.WriteBoolean(false);
            //    }

            //    base.WriteInteger(0);
            //    base.WriteBoolean(true);
            //}


            //base.WriteInteger(0);
            //{
            //    //int, bool, int, bool
            //    base.WriteInteger(3253248);


            //    base.WriteBoolean(false);
            //    base.WriteInteger(256);
            //    base.WriteBoolean(false);
            //    base.WriteInteger(0);
            //    base.WriteBoolean(true);


            //}
        }
    }
}
