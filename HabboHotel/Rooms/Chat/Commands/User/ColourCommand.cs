using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.Users;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class ColourCommand : IChatCommand
    {

        public string PermissionRequired
        {
            get { return "command_colour"; }
        }
        public string Parameters
        {
            get { return ""; }
        }
        public string Description
        {
            get { return "off/red/green/blue/purple"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Você deve selecionar a cor que deseja.");
                return;
            }
            string chatColour = Params[1];
            string Colour = chatColour.ToLower();
            switch (chatColour)
            {
                case "none":
                case "black":
                case "off":
                    Session.GetHabbo().chatColour = "";
                    Session.SendWhisper("Seu bate-papo colorido foi desabilitado.");
                    break;
                case "blue":
                case "red":
                case "cyan":
                case "purple":
                case "green":
                    Session.GetHabbo().chatColour = chatColour;
                    Session.SendWhisper("@"+ Colour +"Você mudou a cor para: " + Colour + "");
                    break;
                default:
                    Session.SendWhisper("A cor: " + Colour + " não existe.");
                    break;
            }
            return;
        }
    }
}
