using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Users.Effects;

namespace Quasar.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    class AvatarEffectActivatedComposer : ServerPacket
    {
        public AvatarEffectActivatedComposer(AvatarEffect Effect)
            : base(ServerPacketHeader.AvatarEffectActivatedMessageComposer)
        {
            base.WriteInteger(Effect.SpriteId);
            base.WriteInteger((int)Effect.Duration);
            base.WriteBoolean(false);//Permanent
        }
    }
}