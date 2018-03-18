using Oreo.HabboHotel.GameClients;
using Oreo.HabboHotel.Rooms;
 
namespace Oreo.HabboHotel.Items.Interactor
{
    class InteractorJukebox : IFurniInteractor
    {
        public Room Room
        {
            get;
            private set;
        }
 
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.GetRoom().GetRoomItemHandler().JukeboxCount++;//quando coloca
        }
 
        public void OnRemove(GameClient Session, Item Item)
        {
            Item.GetRoom().GetRoomItemHandler().JukeboxCount--;//quando tira 
            Item.ExtraData = "0";
            Item.UpdateState();
        }
 
        public void OnWiredTrigger(Item Item)
        {
            if (Item.GetRoom().GetTraxManager().IsPlaying)
                Item.GetRoom().GetTraxManager().StopPlayList();
            else
                Item.GetRoom().GetTraxManager().PlayPlaylist();
        }
 
        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            Room room = Item.GetRoom();
            bool flag = Request == 0 || Request == 1;
            if (flag)
            {
                room.GetTraxManager().TriggerPlaylistState();
            }
        }
    }
}