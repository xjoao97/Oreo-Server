/*﻿using Quasar.HabboHotel.GameClients;
using Quasar.Messages.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Quasar.HabboHotel;
using Quasar.Communication.Packets.Outgoing.Avatar;
using Atlanta.Protocol.Messages.outgoing;

namespace Quasar.HabboHotel.Alphas
{
    class AlphaManager
    {
        private static AlphaManager instance = new AlphaManager();

        internal static AlphaManager getInstance()
        {
            return instance;
        }

        private Thread thread;
        private bool threadActive;

        internal Dictionary<uint, Alpha> onService;
        internal Dictionary<uint, Alpha> onPending;
        internal Stopwatch timer;

        internal AlphaManager()
        {
            this.onService = new Dictionary<uint, Alpha>();
            this.onPending = new Dictionary<uint, Alpha>();
            this.timer = new Stopwatch();
            this.timer.Start();
        }

        internal void onRequest(string message, GameClient Session)
        {
            if (Session.GetHabbo() == null) return;

            var client = searchHelper();
            if (client == null)
            {
                Session.SendMessage(onGuideSessionError.composer(2));
                return;
            }

            if (Session.GetHabbo().rankHelper != Users.typeOfHelper.None)
            {
                Session.SendMessage(onGuideSessionError.composer(0));
                return;
            }

            if (client != null && Session.GetHabbo().Id != client.GetHabbo().Id)
            {
                Session.SendMessage(onGuideSessionAttached.composer(false, message));
                client.SendMessage(onGuideSessionAttached.composer(true, message));
                client.GetHabbo().onService = true;

                uint twoId = client.GetHabbo().Id;

                Session.GetHabbo().userHelping = twoId;
                if (!onPending.ContainsKey(twoId))
                    onPending.Add(twoId, new Alpha(client, Session, message));
            }
            else
            {
                Session.SendMessage(onGuideSessionError.composer(2));
            }
        }

        internal void onClose(uint id)
        {
            if (onService.ContainsKey(id))
            {
                var alpha = onService[id];

                alpha.userrequest.SendMessage(new onGuideSessionDetached());
                alpha.helper.SendMessage(new onGuideSessionDetached);
                var sendMessage = onGuideSessionEnded.composer(2);
                alpha.helper.SendMessage(sendMessage);
                alpha.userrequest.SendMessage(sendMessage);
                alpha.userrequest.GetHabbo().userHelping = 0;
                alpha.helper.GetHabbo().onService = false;
                onService.Remove(id);
            }
        }

        internal void onChat(uint id, string message, bool isMy)
        {
            if (onService.ContainsKey(id))
            {
                var alpha = onService[id];
                var toId = isMy ? alpha.userrequest.GetHabbo().Id : alpha.helper.GetHabbo().Id;
                alpha.helper.SendMessage(onGuideSessionMessage.composer(toId, message));
                alpha.userrequest.SendMessage(onGuideSessionMessage.composer(toId, message));
            }
        }

        internal void onCancelRequest(uint id)
        {
            if (onPending.ContainsKey(id))
            {
                var alpha = onPending[id];
                alpha.helper.GetHabbo().onService = false;
                alpha.userrequest.SendMessage(new onGuideSessionDetached);
                alpha.helper.SendMessage(new onGuideSessionDetached);
                alpha.userrequest.GetHabbo().userHelping = 0;
                onPending.Remove(id);
            }
        }

        internal void joinToRoom(uint userId)
        {
            if (onService.ContainsKey(userId))
            {
                var alpha = onService[userId];
                var currentRoomUser = alpha.userrequest.GetHabbo().CurrentRoomId;
                alpha.helper.SendMessage(onGuideSessionRequesterRoom.composer(currentRoomUser));
            }
        }

        internal void PartnerIsTyping(uint Id, bool typing, bool helper)
        {
            if (onService.ContainsKey(Id))
            {
                var alpha = onService[Id];
                if (helper)
                    alpha.helper.SendMessage(onGuideSessionPartnerIsTyping.composer(typing));
                else
                    alpha.userrequest.SendMessage(onGuideSessionPartnerIsTyping.composer(typing));
            }
        }

        internal void onInvitedToGuideRoom(uint Id)
        {
            if (onService.ContainsKey(Id))
            {
                var alpha = onService[Id];
                var currentAlphaRoom = alpha.helper.GetHabbo().CurrentRoom;

                if (currentAlphaRoom != null)
                    alpha.userrequest.SendMessage(onGuideSessionInvitedToGuideRoom.composer(currentAlphaRoom.RoomId, currentAlphaRoom.RoomData.Name));
            }
        }

        internal void onAccepted(uint Id)
        {
            if (onPending.ContainsKey(Id))
            {
                var alpha = onPending[Id];
                alpha.helper.GetHabbo().onService = true;
                alpha.userrequest.GetHabbo().userHelping = Id;

                alpha.helper.SendMessage(new onGuideSessionStarted(alpha.userrequest.GetHabbo(), alpha.helper.GetHabbo()));
                alpha.userrequest.SendMessage(new onGuideSessionStarted(alpha.userrequest.GetHabbo(), alpha.helper.GetHabbo()));

                if (!onService.ContainsKey(Id))
                    onService.Add(Id, alpha);


                onPending.Remove(Id);

            }

        }

        internal void onDeclined(uint id)
        {
            if (onPending.ContainsKey(id))
            {
                var alpha = onPending[id];
                alpha.userrequest.SendMessage(new onGuideSessionDetached);
                alpha.userrequest.SendMessage(onGuideSessionError.composer(1));
                alpha.helper.SendMessage(new onGuideSessionDetached);
                QuasarEnvironment.GetGame().GetClientManager().modifyGuide(false, alpha.helper);
                alpha.helper.GetHabbo().onDuty = false;
                alpha.helper.GetHabbo().requestTour = false;
                alpha.helper.GetHabbo().requestHelp = false;
                alpha.helper.GetHabbo().reportsOfHarassment = false;
                alpha.helper.SendMessage(helpGuideTool.composer(false));
                onPending.Remove(id);

            }
        }

        List<uint> toRemove = new List<uint>();
        internal void onCycle()
        {
            try
            {
                if (timer.ElapsedMilliseconds >= 70000)
                {
                    timer.Restart();
                    toRemove.Clear();
                    foreach (var pending in onPending)
                    {
                        if (pending.Value.helper.GetHabbo() == null)
                        {
                            pending.Value.userrequest.SendMessage(new onGuideSessionDetached);
                            toRemove.Add(pending.Key);
                            continue;
                        }

                        if (pending.Value.userrequest.GetHabbo() == null)
                        {
                            pending.Value.helper.SendMessage(new onGuideSessionDetached);
                            toRemove.Add(pending.Key);
                            continue;
                        }

                        if (!pending.Value.helper.GetHabbo().onDuty)
                        {
                            pending.Value.helper.SendMessage(new onGuideSessionDetached);
                            pending.Value.userrequest.SendMessage(new onGuideSessionDetached);
                            toRemove.Add(pending.Key);
                            continue;
                        }
                    }

                    foreach (var service in onService)
                    {
                        if (service.Value.helper.GetHabbo() == null)
                        {
                            service.Value.userrequest.SendMessage(new onGuideSessionDetached);
                            toRemove.Add(service.Key);
                            continue;
                        }

                        if (service.Value.userrequest.GetHabbo() == null)
                        {
                            service.Value.helper.SendMessage(new onGuideSessionDetached);
                            toRemove.Add(service.Key);
                            continue;
                        }

                        if (!service.Value.helper.GetHabbo().onDuty)
                        {
                            service.Value.helper.SendMessage(new onGuideSessionDetached);
                            service.Value.userrequest.SendMessage(new onGuideSessionDetached);
                            toRemove.Add(service.Key);
                            continue;
                        }
                    }

                    foreach (var remove in toRemove)
                    {
                        onPending.Remove(remove);
                        onService.Remove(remove);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        internal GameClient searchHelper()
        {
            List<GameClient> list = new List<GameClient>();
            foreach (GameClient guide in QuasarEnvironment.GetGame().GetClientManager().getGuides().Values)
            {
                if (guide.GetHabbo() == null || guide.GetHabbo().rankHelper == Users.typeOfHelper.None) continue;
                if (guide.GetHabbo().requestHelp)
                    list.Add(guide);
            }

            if (list.Count == 0) return null;


            return list[QuasarEnvironment.GetRandomNumber(0,list.Count-1)];
        }
    }

    {
        internal GameClient helper, userrequest;
        internal string message;
        internal Alpha(GameClient helper, GameClient userrequest, string message)
        {
            this.helper = helper;
            this.userrequest = userrequest;
            this.message = message;
        }
    }
}*/
