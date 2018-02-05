using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulator.HabboHotel.Rooms
{
   public class VisitedRooms : RoomData
    {
        public int UserId;
        public int RoomId;
    
        public VisitedRooms(int uid, int rid)
        {
            this.UserId = uid;
            this.RoomId = rid;
        }   
        

    }
}