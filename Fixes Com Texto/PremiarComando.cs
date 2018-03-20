using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
using Oreo.HabboHotel.Rooms;
using Oreo.HabboHotel.Pathfinding;
using Oreo.HabboHotel.GameClients;
using Oreo.Communication.Packets.Outgoing.Rooms.Chat;
using Oreo.Communication.Packets.Outgoing.Rooms.Notifications;
using Oreo.Communication.Packets.Outgoing.Inventory.Purse;
using Oreo.Database.Interfaces;
using System.Data;
using Oreo.Communication.Packets.Outgoing.Users;
using Oreo.HabboHotel.Quests;
using Oreo.Core;
 
namespace Oreo.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class PremiarCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_premiar"; }
        }
 
        public string Parameters
        {
            get { return "%username%"; }
        }
 
        public string Description
        {
            get { return "Faz todas as funções para premia um ganhador de evento."; }
        }
 
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
 
 
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o usuário que deseja premiar!", 1);
                return;
            }
 
            GameClient Target = OreoServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Opa, não foi possível encontrar esse usuário!");
                return;
            }
 
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Target.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("Usuário não encontrado! Talvez ele não esteja online ou nesta sala.");
                return;
            }
 
            if (Target.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Você não pode se premiar!");
                return;
            }
 
 
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }
            else
            {
 
 
                Target.GetHabbo().Credits = Target.GetHabbo().Credits += Convert.ToInt32(OreoServer.GetConfig().data["Moedaspremiar"]);
                Target.SendMessage(new CreditBalanceComposer(Target.GetHabbo().Credits));
                Target.GetHabbo().Duckets += Convert.ToInt32(OreoServer.GetConfig().data["Ducketspremiar"]);
                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, 500));
                Target.GetHabbo().Diamonds += Convert.ToInt32(OreoServer.GetConfig().data["Diamantespremiar"]);
                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, 50, 5));
                Target.GetHabbo().GOTWPoints += Convert.ToInt32(OreoServer.GetConfig().data["Gotwspremiar"]);
                Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, 1, 103));
 
 
                Target.SendMessage(new RoomNotificationComposer("moedas", "message", "Você ganhou " + Convert.ToInt32(OreoServer.GetConfig().data["Diamantespremiar"]) + " diamantes, parabéns " + Target.GetHabbo().Username + "!"));
 
 
                if (Session.GetHabbo().Rank >= 0)
                {
                    DataRow nivel = null;
                    using (var dbClient = OreoServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT Premiar FROM users WHERE id = '" + Target.GetHabbo().Id + "'");
                        nivel = dbClient.getRow();
                        dbClient.RunQuery("UPDATE users SET Premiar = Premiar + '1' WHERE id = '" + Target.GetHabbo().Id + "'");
                        dbClient.RunQuery("UPDATE users SET pontos_evento = pontos_evento = '1' WHERE id = '" + Target.GetHabbo().Id + "'");
                        dbClient.RunQuery();
                    }
                    if (Convert.ToString(nivel["Premiar"]) != OreoServer.GetConfig().data["NiveltotalGames"])
                    {
                        string emblegama = "NV" + Convert.ToString(nivel["Premiar"]);
 
                        if (!Target.GetHabbo().GetBadgeComponent().HasBadge(emblegama))
                        {
                            Target.GetHabbo().GetBadgeComponent().GiveBadge(emblegama, true, Target);
                            if (Target.GetHabbo().Id != Session.GetHabbo().Id)
                                Target.SendMessage(new RoomNotificationComposer("badge/" + emblegama, 3, "Você acaba de receber um emblema gamer de nivel: " + emblegama + " !", ""));
 
                            OreoServer.GetGame().GetAchievementManager().ProgressAchievement(Target, "ACH_Evento", 1);
 
                            string figure = Target.GetHabbo().Look;
                            OreoServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("fig/" + figure, 3, TargetUser.GetUsername() + " ganhou um evento no hotel. Parabéns!", " Nivel do emblema gamer: NIVEL " + Convert.ToString(nivel["Premiar"]) + " !"));
                        }
                        else
                            Session.SendWhisper("Ops, ocorreu um erro no sistema de dar emblemas automáticos! Erro no emblema: (" + emblegama + ") !");
 
                        Session.SendWhisper("Comando (Premiar) realizado com sucesso!");
                    }
                }
            }
        }
 
        private void SendMessage(RoomNotificationComposer roomNotificationComposer)
        {
            throw new NotImplementedException();
        }
    }
}