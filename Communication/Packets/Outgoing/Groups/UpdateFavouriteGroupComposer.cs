using System;
using System.Linq;
using System.Text;

using Quasar.HabboHotel.Groups;

namespace Quasar.Communication.Packets.Outgoing.Groups
{
    class UpdateFavouriteGroupComposer : ServerPacket
    {
        public UpdateFavouriteGroupComposer(int Id, Group Group, int VirtualId)
            : base(ServerPacketHeader.UpdateFavouriteGroupMessageComposer)
        {
            base.WriteInteger(VirtualId);//Sends 0 on .COM
            base.WriteInteger(Group != null ? Group.Id : 0);
            base.WriteInteger(3);
            base.WriteString(Group != null ? Group.Name : string.Empty);
        }
    }
}
