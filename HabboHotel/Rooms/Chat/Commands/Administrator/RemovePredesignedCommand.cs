using Quasar.HabboHotel.Catalog.PredesignedRooms;
using System.Text;
using System.Linq;
using System.Globalization;

namespace Quasar.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class RemovePredesignedCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_removepredesigned"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Retire o quarto da lista de quartos"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null) return;
            //if (!QuasarEnvironment.GetGame().GetCatalog().GetPredesignedRooms().Exists((uint)Room.Id))
            //{
            //    Session.SendWhisper("O quarto não existe na lista.");
            //    return;
            //}

            var predesignedId = 0U;
            using (var dbClient = QuasarEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id FROM catalog_predesigned_rooms WHERE room_id = " + Room.Id + ";");
                predesignedId = (uint)dbClient.getInteger();

                dbClient.runFastQuery("DELETE FROM catalog_predesigned_rooms WHERE room_id = " + Room.Id + " AND id = " +
                    predesignedId + ";");
            }

            QuasarEnvironment.GetGame().GetCatalog().GetPredesignedRooms().predesignedRoom.Remove(predesignedId);
            Session.SendWhisper("O quarto foi excluído com sucesso da lista.");
        }
    }
}
