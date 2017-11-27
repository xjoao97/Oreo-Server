﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Items;


using Quasar.Communication.Packets.Outgoing.Inventory.Furni;
using Quasar.Database.Interfaces;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class PickAllCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_pickall"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Recolhe tudo que tem no quarto."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            Room.GetRoomItemHandler().RemoveItems(Session);
            Room.GetGameMap().GenerateMaps();

            using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `room_id` = @RoomId AND `user_id` = @UserId");
                dbClient.AddParameter("RoomId", Room.Id);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            List<Item> Items = Room.GetRoomItemHandler().GetWallAndFloor.ToList();
            if (Items.Count > 0)
                Session.SendWhisper("Se ainda sobrar items no quarto recolha manualmente ou digite :ejectall");

            Session.SendMessage(new FurniListUpdateComposer());
        }
    }
}
