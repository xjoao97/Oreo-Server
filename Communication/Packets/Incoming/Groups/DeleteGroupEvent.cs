﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.Groups;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

using Quasar.HabboHotel.Rooms;

namespace Quasar.Communication.Packets.Incoming.Groups
{
    class DeleteGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Group Group = null;
            if (!QuasarEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group))
            {
                Session.SendMessage(new RoomNotificationComposer("Oops!",
                 "Grupo não encontrado", "nothing", ""));
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("group_delete_override"))
            {
                Session.SendMessage(new RoomNotificationComposer("Oops!",
                 "Apenas o dono pode excluir esse grupo", "nothing", ""));
                return;
            }

            if (Group.MemberCount >= QuasarStaticGameSettings.GroupMemberDeletionLimit && !Session.GetHabbo().GetPermissions().HasRight("group_delete_limit_override"))
            {
                Session.SendMessage(new RoomNotificationComposer("Sucesso",
                 "O grupo atingiu o limite de membros(" + QuasarStaticGameSettings.GroupMemberDeletionLimit + "), entre em contato com um dos membros da equipe administrativa!", "nothing", ""));
                return;
            }

            Room Room = QuasarEnvironment.GetGame().GetRoomManager().LoadRoom(Group.RoomId);

            if (Room != null)
            {
                Room.Group = null;
                Room.RoomData.Group = null;
            }

            QuasarEnvironment.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `groups` WHERE `id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Group.Id + "'");
            }

            QuasarEnvironment.GetGame().GetRoomManager().UnloadRoom(Room, true);

            Session.SendMessage(new RoomNotificationComposer("Sucesso,",
                 "Você excluiu o grupo com sucesso!", "nothing", ""));
            return;
        }
    }
}
