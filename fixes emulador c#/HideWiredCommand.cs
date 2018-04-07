using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Oreo.Communication.Packets.Outgoing.Inventory.Furni;
using System.Globalization;
using Oreo.Database.Interfaces;
using Oreo.Communication.Packets.Outgoing;
using Oreo.HabboHotel.Items;
using Oreo.Communication.Packets.Outgoing.Rooms.Engine;
using Oreo.Communication.Packets.Outgoing.Rooms.Chat;

namespace Oreo.HabboHotel.Rooms.Chat.Commands.User
{
    class HideWiredCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Esconder Wired No seu quarto ."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (!Room.CheckRights(Session, false, false))
            {
                Session.SendWhisper("Você não tem direitos neste quarto!");
                return;
            }

            Room.HideWired = !Room.HideWired;
            if (Room.HideWired)
                Session.SendWhisper("Wired foi escondido.");
            else
                Session.SendWhisper("Wired foi mostrado.");

            using (IQueryAdapter con = OreoServer.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("UPDATE `rooms` SET `hide_wired` = @enum WHERE `id` = @id LIMIT 1");
                con.AddParameter("enum", OreoServer.BoolToEnum(Room.HideWired));
                con.AddParameter("id", Room.Id);
                con.RunQuery();
            }

            List<ServerPacket> list = new List<ServerPacket>();

            list = Room.HideWiredMessages(Room.HideWired);

            Room.SendMessage(list);


        }
    }
}
