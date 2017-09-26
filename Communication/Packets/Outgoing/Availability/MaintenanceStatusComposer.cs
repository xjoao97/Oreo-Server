namespace Quasar.Communication.Packets.Outgoing.Availability
{
    class MaintenanceStatusComposer : ServerPacket
    {
        public MaintenanceStatusComposer(int Minutes, int Duration)
            : base(ServerPacketHeader.MaintenanceStatusMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(Minutes);
            base.WriteInteger(Duration);
        }
    }
}
