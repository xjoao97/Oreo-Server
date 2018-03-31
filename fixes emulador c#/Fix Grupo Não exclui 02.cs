using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Oreo.Database.Interfaces;
using Oreo.HabboHotel.Groups;
using Oreo;
using Oreo.HabboHotel.Rooms;

namespace Oreo.Communication.Packets.Incoming.Groups
{
    class DeleteGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!OreoServer.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group Group))
            {
                Session.SendNotification("Ops! Não conseguimos encontrar o grupo");
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("group_delete_override"))//Maybe a FUSE check for staff override?
            {
                Session.SendNotification("Ops! só o dono pode excluir!");
                return;
            }



            if (Group.MemberCount >= Convert.ToInt32(OreoServer.GetGame().GetSettingsManager().TryGetValue("group.delete.member.limit")) && !Session.GetHabbo().GetPermissions().HasRight("group_delete_limit_override"))
            {
                Session.SendNotification("Ops seu grupo já superrous a quantidade maxima  de mebros! (" + Convert.ToInt32(OreoServer.GetGame().GetSettingsManager().TryGetValue("group.delete.member.limit")) + ") que un grupo puede exceder antes de ser elegible para su eliminación. Solicitar asistencia de un miembro del staff.");
                return;
            }

            Room Room = OreoServer.GetGame().GetRoomManager().LoadRoom(Group.RoomId);

            if (Room != null)
            {
                Room.Group = null;
                Room.RoomData.Group = null;//I'm not sure if this is needed or not, becauseof inheritance, but oh well.
            }

            //Remove it from the cache.
            OreoServer.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            //Now the :S stuff.
            using (IQueryAdapter dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM `groups` WHERE `id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.RunQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Group.Id + "' LIMIT 1");
                dbClient.RunQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Group.Id + "'");
            }

            //Unload it last.
            OreoServer.GetGame().GetRoomManager().UnloadRoom(Room, true);

            //Say hey!
            Session.SendNotification("Eliminou corretamente o grupo.");
            return;
        }
    }
}