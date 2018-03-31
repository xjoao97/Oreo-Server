using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Oreo.Database.Interfaces;
using Oreo.Communication.Packets.Outgoing.Messenger;
using Oreo.Communication.Packets.Outgoing.Moderation;

namespace Oreo.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class DeleteGroupCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_delete_group"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Apaga um grupo do banco de dados e do hotel."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (Room.Group == null)
            {
                Session.SendWhisper("Bem, não há nenhum grupo aqui?");
                return;
            }

            using (IQueryAdapter dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `groups` WHERE `id` = '" + Room.Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Room.Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Room.Group.Id + "'");
                dbClient.RunQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Room.Group.Id + "' LIMIT 1");
                dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Room.Group.Id + "' LIMIT 1");
                dbClient.RunQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Room.Group.Id + "'");
            }

            OreoServer.GetGame().GetGroupManager().DeleteGroup(Room.RoomData.Group.Id);

            Room.Group = null;
            Room.RoomData.Group = null;

            OreoServer.GetGame().GetRoomManager().UnloadRoom(Room, true);
            if (Room.RoomData.Group.HasChat)
            {
                var Client = OreoServer.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().Id);
                if (Client != null)
                {
                    Client.SendMessage(new FriendListUpdateComposer(Room.RoomData.Group, -1));
                }
            }

            Session.SendNotification("êxito, grupo eliminado.");
            return;
        }
    }
}