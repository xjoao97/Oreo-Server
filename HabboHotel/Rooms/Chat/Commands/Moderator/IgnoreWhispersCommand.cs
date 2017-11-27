using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class IgnoreWhispersCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_ignore_whispers"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Isso permite que você ignore todos os sussuros na sala"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().IgnorePublicWhispers = !Session.GetHabbo().IgnorePublicWhispers;
            Session.SendWhisper("Você " + (Session.GetHabbo().IgnorePublicWhispers ? "está" : "não está") + " ignorando os sussuros!");
        }
    }
}
