using System;
using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Rooms;
using Quasar.Communication.Packets.Outgoing;
using Quasar.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks;
using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.Database.Interfaces;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni.LoveLocks
{
    class ConfirmLoveLockEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int pId = Packet.PopInt();
            bool isConfirmed = Packet.PopBoolean();

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            Item Item = Room.GetRoomItemHandler().GetItem(pId);

            if (Item == null || Item.GetBaseItem() == null || Item.GetBaseItem().InteractionType != InteractionType.LOVELOCK)
                return;

            int UserOneId = Item.InteractingUser;
            int UserTwoId = Item.InteractingUser2;

            RoomUser UserOne = Room.GetRoomUserManager().GetRoomUserByHabbo(UserOneId);
            RoomUser UserTwo = Room.GetRoomUserManager().GetRoomUserByHabbo(UserTwoId);

            if(UserOne == null && UserTwo == null)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                Session.SendNotification("Seu parceiro saiu ou cancelou o fechamento!");
                return;
            }
            else if(UserOne.GetClient() == null || UserTwo.GetClient() == null)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                Session.SendNotification("Seu parceiro saiu ou cancelou o fechamento!");
                return;
            }
            else if(UserOne == null)
            {
                UserTwo.CanWalk = true;
                UserTwo.GetClient().SendNotification("Seu parceiro saiu ou cancelou o fechamento!");
                UserTwo.LLPartner = 0;
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if(UserTwo == null)
            {
                UserOne.CanWalk = true;
                UserOne.GetClient().SendNotification("Seu parceiro saiu ou cancelou o fechamento!");
                UserOne.LLPartner = 0;
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if(Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
            {
                UserTwo.CanWalk = true;
                UserTwo.GetClient().SendNotification("Esse cadeado já foi fechado!");
                UserTwo.LLPartner = 0;

                UserOne.CanWalk = true;
                UserOne.GetClient().SendNotification("Esse cadeado já foi fechado!");
                UserOne.LLPartner = 0;

                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;
                return;
            }
            else if(!isConfirmed)
            {
                Item.InteractingUser = 0;
                Item.InteractingUser2 = 0;

                UserOne.LLPartner = 0;
                UserTwo.LLPartner = 0;

                UserOne.CanWalk = true;
                UserTwo.CanWalk = true;
                return;
            }
            else
            {
                if(UserOneId == Session.GetHabbo().Id)
                {
                    Session.SendMessage(new LoveLockDialogueSetLockedMessageComposer(pId));
                    UserOne.LLPartner = UserTwoId;
                }
                else if(UserTwoId == Session.GetHabbo().Id)
                {
                    Session.SendMessage(new LoveLockDialogueSetLockedMessageComposer(pId));
                    UserTwo.LLPartner = UserOneId;
                }

                if (UserOne.LLPartner == 0 || UserTwo.LLPartner == 0)
                    return;
                else
                {
                    Item.ExtraData = "1" + (char)5 + UserOne.GetUsername() + (char)5 + UserTwo.GetUsername() + (char)5 + UserOne.GetClient().GetHabbo().Look + (char)5 + UserTwo.GetClient().GetHabbo().Look + (char)5 + DateTime.Now.ToString("dd/MM/yyyy");

                    Item.InteractingUser = 0;
                    Item.InteractingUser2 = 0;

                    UserOne.LLPartner = 0;
                    UserTwo.LLPartner = 0;

                    Item.UpdateState(true, true);

                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE items SET extra_data = @extraData WHERE id = @ID LIMIT 1");
                        dbClient.AddParameter("extraData", Item.ExtraData);
                        dbClient.AddParameter("ID", Item.Id);
                        dbClient.RunQuery();
                    }

                    UserOne.GetClient().SendMessage(new LoveLockDialogueCloseMessageComposer(pId));
                    UserTwo.GetClient().SendMessage(new LoveLockDialogueCloseMessageComposer(pId));

                    UserOne.CanWalk = true;
                    UserTwo.CanWalk = true;

                    UserOne = null;
                    UserTwo = null;
                }
            }
        }
    }
}
