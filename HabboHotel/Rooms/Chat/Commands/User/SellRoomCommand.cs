using Quasar.Communication.Packets.Outgoing.Rooms.Notifications;
using Quasar.HabboHotel.GameClients;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class SellRoomCommand : IChatCommand
    {
        public string Description
        {
            get { return "Poner en venta la sala en que te encuentras."; }
        }

        public string Parameters
        {
            get { return "%precio%"; }
        }

        public string PermissionRequired
        {
            get { return "command_sell_room"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            if (Session.GetHabbo()._sellingroom == false)
            {
                Session.SendMessage(new RoomNotificationComposer("Alerta sobre las ventas de sala:", "<font color = '#B40404'><b>¡ATENCIÓN!</b></font>\n\n<font size ='11' color='#1C1C1C'>Estás a punto de poner en venta tu sala.\n" +
                 "Para confirmar esta renta, escribe :sellroom + PRECIO EN DUCKETS (ejemplo) <font color='#B40404'> <b>:sellroom 500</b></font>. \n\nUna vez hagas esto pondrás tu sala en venta. Recuerda que puedes revertir la acción escribiendo :unload.\n\n<font color='#B40404'><i>" + QuasarEnvironment.GetDBConfig().DBData["hotel.name"] + " no se hará responsable de timos con esto ya que acabas de ser advertido de que tu sala va a ponerse en venta.</i></font>\n\n" +
                 "Esperamos que disfrutes de tu venta, ¡buena suerte con los postores!", "sellroom", ""));
                Session.GetHabbo()._sellingroom = true;
                return;
            }

            if (Room == null)


                if (Params.Length == 1)
                {
                    Session.SendWhisper("Oops, se olvidó de elegir un precio para vender esta sala.", 34);
                    return;
                }
                else if (Room.Group != null)
                {
                    Session.SendWhisper("Oops, al parecer esta sala tiene un grupo, así no puedes venderla, primero debes eliminar el grupo.", 34);
                    return;
                }

            int Value = 0;
            if (!int.TryParse(Params[1], out Value))
            {
                Session.SendWhisper("Oops, estas introduciendo un valor que no es correcto.", 34);
                return;
            }

            if (Value < 0)
            {
                Session.SendWhisper("No puede vender una sala con un valor numérico negativo.", 34);
                return;
            }

            if (Room.ForSale)
            {
                Room.SalePrice = Value;
            }
            else
            {
                Room.ForSale = true;
                Room.SalePrice = Value;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (User == null || User.GetClient() == null)
                    continue;

                Session.SendWhisper("¡Esta sala está en venta, su precio actual es de:  " + Value + " Duckets! Cómprala escribiendo :buyroom.");
            }

            Session.SendNotification("Si usted quiere vender su sala, debe incluir un valor numerico. \n\nPOR FAVOR NOTA:\nSi usted vende una sala, no la puede Recuperar de nuevo.!\n\n" +
            "Usted puede cancelar la venta de una habitación escribiendo ':unload' (sin las '')");

            return;
        }

    }
}