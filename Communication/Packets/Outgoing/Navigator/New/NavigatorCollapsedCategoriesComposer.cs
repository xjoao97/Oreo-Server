using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.Communication.Packets.Outgoing.Navigator
{
    class NavigatorCollapsedCategoriesComposer : ServerPacket
    {
        public NavigatorCollapsedCategoriesComposer()
            : base(ServerPacketHeader.NavigatorCollapsedCategoriesMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
