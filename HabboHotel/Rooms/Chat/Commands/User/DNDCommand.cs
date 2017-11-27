using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class DNDCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_dnd"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ative ou desative as mensagens do console."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().AllowConsoleMessages = !Session.GetHabbo().AllowConsoleMessages;
            Session.SendWhisper("Usted " + (Session.GetHabbo().AllowConsoleMessages == true ? "ahora" : "ya no") + " acepta mensajes en su consola de amigos.");
        }
    }
}
