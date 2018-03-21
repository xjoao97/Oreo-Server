using System;
using System.Threading;
using log4net;
using Emulator.Database.Interfaces;

namespace Emulator.HabboHotel.Global
{
    public class ServerStatusUpdater : IDisposable
    {
        private static ILog log = LogManager.GetLogger("Mango.Global.ServerUpdater");

        private const int UPDATE_IN_SECS = 30;

        private Timer _timer;
        
        public ServerStatusUpdater()
        {
        }

        public void Init()
        {
            this._timer = new Timer(new TimerCallback(this.OnTick), null, TimeSpan.FromSeconds(UPDATE_IN_SECS), TimeSpan.FromSeconds(UPDATE_IN_SECS));

            Console.Title = "Habbo Emulator ~ the whole new revolution.";

            log.Info("Server Status Updater has been started.");
        }

        public void OnTick(object Obj)
        {
            this.UpdateOnlineUsers();
            
        }

        private void UpdateOnlineUsers()
        {         
            TimeSpan Uptime = DateTime.Now - HabboEnvironment.ServerStarted;
            int UsersOnline = Convert.ToInt32(HabboEnvironment.GetGame().GetClientManager().Count);
            int RoomCount = HabboEnvironment.GetGame().GetRoomManager().Count;
            int userPeak = HabboEnvironment.GetGame().GetClientManager().GetUserPeak();
            
            
            if(UsersOnline > userPeak)
            {
                using (IQueryAdapter dbClient = HabboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `server_status` SET `user_peak` = @peak LIMIT 1;");
                    dbClient.AddParameter("peak", UsersOnline);                
                    dbClient.RunQuery();
                }
                log.Info("New User Peak Achieved: "+ UsersOnline +" Users Online, congratulations!");
            }
            
            Console.Title = "Habbo Emulator ~ the whole new revolution.";

            using (IQueryAdapter dbClient = HabboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `server_status` SET `users_online` = @users, `loaded_rooms` = @loadedRooms LIMIT 1;");
                dbClient.AddParameter("users", UsersOnline);
                dbClient.AddParameter("loadedRooms", RoomCount);
                dbClient.RunQuery();
            }
        }


        public void Dispose()
        {
            using (IQueryAdapter dbClient = HabboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `server_status` SET `users_online` = '0', `loaded_rooms` = '0'");
            }

            this._timer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
