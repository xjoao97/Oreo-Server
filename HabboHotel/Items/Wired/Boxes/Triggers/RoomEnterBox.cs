using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;
using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Triggers
{
    class RoomEnterBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.TriggerRoomEnter; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public RoomEnterBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            string User = Packet.PopString();

            this.StringData = User;
        }

        public bool Execute(params object[] Params)
        {
            Instance.GetWired().OnEvent(Item);

            Habbo Player = (Habbo)Params[0];

            if (!string.IsNullOrWhiteSpace(StringData) && Player.Username != StringData)
                return false;

            ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(this);
            ICollection<IWiredItem> Conditions = Instance.GetWired().GetConditions(this);

            foreach (IWiredItem Condition in Conditions)
            {
                if (!Condition.Execute(Player))
                    return false;

                Instance.GetWired().OnEvent(Condition.Item);
            }

            bool HasRandomEffectAddon = Effects.Where(x => x.Type == WiredBoxType.AddonRandomEffect).ToList().Count() > 0;
            if (HasRandomEffectAddon)
            {
                IWiredItem RandomBox = Effects.FirstOrDefault(x => x.Type == WiredBoxType.AddonRandomEffect);
                if (!RandomBox.Execute())
                    return false;

                IWiredItem SelectedBox = Instance.GetWired().GetRandomEffect(Effects.ToList());
                if (!SelectedBox.Execute())
                    return false;

                if (Instance != null)
                {
                    Instance.GetWired().OnEvent(RandomBox.Item);
                    Instance.GetWired().OnEvent(SelectedBox.Item);
                }
            }
            else
            {
                foreach (IWiredItem Effect in Effects)
                {
                    if (!Effect.Execute(Player))
                        return false;

                    Instance.GetWired().OnEvent(Effect.Item);
                }
            }

            return true;
        }
    }
}
