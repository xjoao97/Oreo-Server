﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Rooms.Games;
using Quasar.HabboHotel.Rooms.Games.Teams;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class EnableCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_enable"; }
        }

        public string Parameters
        {
            get { return "%EffectId%"; }
        }

        public string Description
        {
            get { return "Habilitar un efecto en tu personaje."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Usted debe escribir un ID Efecto");
                return;
            }

            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
                return;

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("No se puede activar un efecto mientras montas un caballo");
                return;
            }
            else if (ThisUser.Team != TEAM.NONE)
                return;
            else if (ThisUser.isLying)
                return;

            int EffectId = 0;
            if (!int.TryParse(Params[1], out EffectId))
                return;

            if (EffectId > int.MaxValue || EffectId < int.MinValue)
                return;

            // Staff Effects
            if (EffectId == 102 && Session.GetHabbo().Rank < 5 || EffectId == 187 && Session.GetHabbo().Rank < 5 || EffectId == 593 && Session.GetHabbo().Rank < 5 || EffectId == 596 && Session.GetHabbo().Rank < 5 || EffectId == 598 && Session.GetHabbo().Rank < 5)
            { Session.SendWhisper("Lo sentimos, lamentablemente sólo los staff pueden activar este efecto.");  return; }

            // Guide Effects
            if (EffectId == 592 && Session.GetHabbo()._guidelevel < 3|| EffectId == 595 && Session.GetHabbo()._guidelevel < 2 || EffectId == 597 && Session.GetHabbo()._guidelevel < 1)
            {  Session.SendWhisper("Lo sentimos, no perteneces al equipo guía, es por ello que no puedes usar este efecto."); return; }

            // Croupier Effect
            if (EffectId == 594 && Session.GetHabbo()._croupier < 1)
            { Session.SendWhisper("Lo sentimos, este enable es sólo para el equipo Croupier de " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + "."); return; }

            // BAW Effect
            if (EffectId == 599 && Session.GetHabbo()._builder < 1)
            { Session.SendWhisper("Lo sentimos, este enable es sólo para el equipo BAW de " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + "."); return; }

            // Publicista Effect
            if (EffectId == 600 && Session.GetHabbo()._publicistalevel < 1)
            {  Session.SendWhisper("Lo sentimos, este enable es sólo para los publicistas."); return; }

            // VIP Effect
            if (EffectId == 44 && Session.GetHabbo().Rank < 2)
            { Session.SendWhisper("Lo sentimos, no eres VIP, por ello no puedes usar dicho enable."); return; }
            
            // Ambassador & Rookies Effect
            if (EffectId == 178 && Session.GetHabbo().Rank < 3)
            { Session.SendWhisper("Lo sentimos, este comando es sólo para los embajadores y rookies."); return; }

            Session.GetHabbo().LastEffect = EffectId;
            Session.GetHabbo().Effects().ApplyEffect(EffectId);
        }
    }
}
