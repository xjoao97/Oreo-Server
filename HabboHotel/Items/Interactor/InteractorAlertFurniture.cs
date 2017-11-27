using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.HabboHotel.Items.Interactor
{
    class InteractorAlertFurniture : IFurniInteractor
    {

        public void OnPlace(GameClient Session, Item Item)
        {
            if (Item.ExtraData == "-1")
            {
                Item.ExtraData = "0";
                Item.UpdateNeeded = true;
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            if (Item.ExtraData == "-1")
            {
                Item.ExtraData = "0";
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            RoomUser User = null;
            if (Session != null)
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
            {
                //if (Item.NextCommand != 0)
                //{
                //    if (Item.NextCommand > QuasarEnvironment.Now())
                //    {
                //        Session.SendWhisper("Advent cooldown", 3);
                //        return;
                //    }
                //}
                //Item.NextCommand = QuasarEnvironment.Now() + 3600000;

                int Amount = 10;
                Session.SendShout("Aha! É o pato do Natal! * Aproveite seus 10 diamantes!");
                Session.GetHabbo().Diamonds += Amount;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, Amount, 5));
                Session.SendNotification("Obrigado por participar da caça de Natal! Você encontrou o pato de Natal raro! Como uma recompensa, você recebeu 10 diamantes!");
                Item.ExtraData = "0";
                Item.UpdateState();


            }
            else
            {
                User.MoveTo(Item.SquareInFront);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
            Item.ExtraData = "-1";
            Item.UpdateState(false, true);
            Item.RequestUpdate(4, true);
        }
    }
}
