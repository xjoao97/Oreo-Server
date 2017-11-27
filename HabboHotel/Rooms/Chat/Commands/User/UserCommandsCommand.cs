using System;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class UserCommandsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_commands"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Comandos do Hotel"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            var _cache = new Random().Next(0, 300);
            Session.SendMessage(new MassEventComposer("habbopages/chat/commands.txt"));
        }
    }
}
