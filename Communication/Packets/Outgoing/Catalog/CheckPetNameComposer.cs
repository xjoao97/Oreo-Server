namespace Quasar.Communication.Packets.Outgoing.Catalog
{
    public class CheckPetNameComposer : ServerPacket
    {
        public CheckPetNameComposer(int Error, string ExtraData)
            : base(ServerPacketHeader.CheckPetNameMessageComposer)
        {
            base.WriteInteger(Error);
           base.WriteString(ExtraData);
        }
    }
}
