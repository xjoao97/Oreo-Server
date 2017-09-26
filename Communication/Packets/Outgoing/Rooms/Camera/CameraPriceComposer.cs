using System;

namespace Quasar.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraPriceComposer : ServerPacket
    {
        public CameraPriceComposer()
            : base(ServerPacketHeader.CameraPriceComposer)
        {
            base.WriteInteger(0);
            base.WriteInteger(0);
            base.WriteInteger(0);
        }
    }
}