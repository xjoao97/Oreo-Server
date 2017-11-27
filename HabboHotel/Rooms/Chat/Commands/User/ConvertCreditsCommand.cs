using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data;

using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Purse;
using Quasar.Database.Interfaces;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class ConvertCreditsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_convert_credits"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Converta suas moedas para carteira"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            int TotalValue = 0;

            try
            {
                DataTable Table = null;
                using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `items` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND (`room_id`=  '0' OR `room_id` = '')");
                    Table = dbClient.getTable();
                }

                if (Table == null)
                {
                    Session.SendWhisper("No momento você não possui moedas em seu inventárioo!");
                    return;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                    if (Item == null)
                        continue;

                    if (!Item.GetBaseItem().ItemName.StartsWith("CF_"))
                        continue;

                    if (Item.RoomId > 0)
                        continue;

                    string[] Split = Item.GetBaseItem().ItemName.Split('_');
                    int Value = int.Parse(Split[1]);

                    using (IQueryAdapter dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

                    TotalValue += Value;

                    if (Value > 0)
                    {
                        Session.GetHabbo().Credits += Value;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                    }
                }

                if (TotalValue > 0)
                    Session.SendNotification("Todos os seus créditos de inventário foram levados para a sua carteira!\r\r(Total: " + TotalValue + " creditos!");
                else
                    Session.SendNotification("Não parece ter nenhum outro item intercambiável!");
            }
            catch
            {
                Session.SendNotification("Oops, Ocorreu um erro ao trocar o seu crédito, entre em contato com um administrador!");
            }
        }
    }
}
