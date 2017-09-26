using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Incoming;
using System.Collections.Concurrent;

using Quasar.Database.Interfaces;
using log4net;
using Quasar.HabboHotel.Items;

namespace Quasar.HabboHotel.Rooms.Music
{
    public class SongInstance
    {
        private readonly SongItem mDiskItem;
        private readonly SongData mSongData;

        public SongInstance(SongItem Item, SongData SongData)
        {
            mDiskItem = Item;
            mSongData = SongData;
        }

        public SongData SongData
        {
            get { return mSongData; }
        }

        public SongItem DiskItem
        {
            get { return mDiskItem; }
        }
    }
}