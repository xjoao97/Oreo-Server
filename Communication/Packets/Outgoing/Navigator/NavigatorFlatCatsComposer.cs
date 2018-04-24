﻿using Quasar.HabboHotel.Navigator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Communication.Packets.Outgoing.Navigator
{
    class NavigatorFlatCatsComposer : ServerPacket
    {
        public NavigatorFlatCatsComposer(ICollection<SearchResultList> Categories, int Rank)
            : base(ServerPacketHeader.NavigatorFlatCatsMessageComposer)
        {
            base.WriteInteger(Categories.Count);
            foreach (SearchResultList Category in Categories)
            {
                base.WriteInteger(Category.Id);
                base.WriteString(Category.PublicName);
                base.WriteBoolean(true);//TODO
            }
        }
    }
}