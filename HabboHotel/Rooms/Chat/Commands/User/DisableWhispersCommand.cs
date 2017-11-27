using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableWhispersCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_whispers"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Habilite ou desabilite a capacidade de receber sussurros."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().ReceiveWhispers = !Session.GetHabbo().ReceiveWhispers;
            Session.SendWhisper("Você " + (Session.GetHabbo().ReceiveWhispers ? "agora" : "agora não") + " recebe susurros!");
        }
    }
}
