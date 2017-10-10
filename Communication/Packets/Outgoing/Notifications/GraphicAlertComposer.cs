namespace Quasar.Communication.Packets.Outgoing.Notifications
{
    class GraphicAlertComposer : ServerPacket
    {
        public GraphicAlertComposer(string image) : base(ServerPacketHeader.GraphicAlertComposer)
        { base.WriteString("${graphic.alerts.images.url}" + image + ".png"); }
    }
}