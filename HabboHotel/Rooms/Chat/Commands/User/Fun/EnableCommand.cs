using System;
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
            get { return "Habilita um efeito especial"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva um número!");
                return;
            }

            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
                return;

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("Você não pode usar um efeito em cima de um cavalo!");
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

            if (EffectId == 102 && Session.GetHabbo().Rank < 5 || EffectId == 187 && Session.GetHabbo().Rank < 5 || EffectId == 593 && Session.GetHabbo().Rank < 5 || EffectId == 596 && Session.GetHabbo().Rank < 5 || EffectId == 598 && Session.GetHabbo().Rank < 5)
            { Session.SendWhisper("Você não é um membro da equipe para usar esse efeito desculpe!");  return; }

            if (EffectId == 592 && Session.GetHabbo()._guidelevel < 3|| EffectId == 595 && Session.GetHabbo()._guidelevel < 2 || EffectId == 597 && Session.GetHabbo()._guidelevel < 1)
            {  Session.SendWhisper("Você não é um membro da equipe para usar esse efeito desculpe."); return; }

            if (EffectId == 594 && Session.GetHabbo()._croupier < 1)
            { Session.SendWhisper("Desculpe, apenas membros da equipe do " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + " podem usar esse efeito."); return; }

            if (EffectId == 599 && Session.GetHabbo()._builder < 1)
            { Session.SendWhisper("Desculpe, apenas membros da equipe do " + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + " podem usar esse efeito."); return; }

            if (EffectId == 600 && Session.GetHabbo()._publicistalevel < 1)
            {  Session.SendWhisper("Você não é um membro da equipe para usar esse efeito desculpe."); return; }

            if (EffectId == 44 && Session.GetHabbo().Rank < 2)
            { Session.SendWhisper("Desculpe, apenas usuários vip podem usar esse efeito!"); return; }

            if (EffectId == 178 && Session.GetHabbo().Rank < 3)
            { Session.SendWhisper("Desculpe, apenas embaixadores podem usar esse efeito."); return; }

            Session.GetHabbo().LastEffect = EffectId;
            Session.GetHabbo().Effects().ApplyEffect(EffectId);
        }
    }
}
