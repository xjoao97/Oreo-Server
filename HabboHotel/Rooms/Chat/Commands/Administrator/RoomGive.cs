﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.GameClients;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RoomGiveCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_room_give"; }
        }

        public string Parameters
        {
            get { return "%type% %amount%"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor introduce ! (coins, duckets, diamonds, pixeles)");
                return;
            }

            string UpdateVal = Params[1];
            switch (UpdateVal.ToLower())
            {
                case "diamonds":
                    {
                        if (Params.Length == 1)
                        {
                            Session.SendWhisper("Introduce el numero de diamantes");
                            return;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))
                                foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
                                {
                                    User.GetClient().GetHabbo().Diamonds += Amount;
                                    User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.GetClient().GetHabbo().Diamonds, Amount, 5));
                                    User.GetClient().SendMessage(new RoomCustomizedAlertComposer(Session.GetHabbo().Username + " te acaba de regalar " + Amount + " Diamantes."));
                                }
                        }
                        Session.SendWhisper("Enviaste correctamente en la sala " + Params[2] + " diamantes!");
                    }
                    break;

                case "reward":
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand("command_roomgive_reward"))
                        {
                            Session.SendWhisper("Oops, No tiene el permiso necesario para usar este comando!");
                            break;
                        }

                        else
                        {
                            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
                            {
                                User.GetClient().SendMessage(QuasarEnvironment.GetGame().GetNuxUserGiftsManager().NuxUserGifts.Serialize());
                            }
                        }
                    }
                    break;

                case "pixeles":
                    {
                        if (Params.Length == 1)
                        {
                            Session.SendWhisper("Introduce el numero de pixeles");
                            return;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[2], out Amount))

                                if (Amount > 50)
                                {
                                    Session.SendWhisper("No pueden enviar más de 50 Pixeles, esto será notificado al CEO y tomará medidas.");
                                    return;
                                }

                            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
                            {
                                User.GetClient().GetHabbo().GOTWPoints += Amount;
                                User.GetClient().SendMessage(new HabboActivityPointNotificationComposer(User.GetClient().GetHabbo().GOTWPoints, Amount, 103));
                                User.GetClient().SendMessage(new RoomCustomizedAlertComposer(Session.GetHabbo().Username + " te acaba de regalar " + Amount + " Pixeles."));
                            }
                        }
                        Session.SendWhisper("Enviaste correctamente en la sala " + Params[2] + " pixeles!");
                    }
                    break;
            }
        }
    }
}
