using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Users;
using Quasar.Communication.Packets.Incoming;
using Quasar.Communication.Packets.Outgoing.Rooms.Chat;
using Quasar.HabboHotel.Rooms.Chat.Commands;

namespace Quasar.HabboHotel.Items.Wired.Boxes.Triggers
{
    class UserSaysCommandBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type { get { return WiredBoxType.TriggerUserSaysCommand; } }
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public UserSaysCommandBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            this.StringData = "";
            this.SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(ClientPacket Packet)
        {
            int Unknown = Packet.PopInt();
            int OwnerOnly = Packet.PopInt();
            string Message = Packet.PopString();

            this.BoolData = OwnerOnly == 1;
            this.StringData = Message;
        }

        public bool Execute(params object[] Params)
        {
            Habbo Player = (Habbo)Params[0];
            if (Player == null || Player.CurrentRoom == null || !Player.InRoom)
                return false;

            RoomUser User = Player.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Player.Username);
            if (User == null)
                return false;

            if ((BoolData && Instance.OwnerId != Player.Id) || string.IsNullOrWhiteSpace(this.StringData))
                return false;

            IChatCommand ChatCommand = null;
            if (!QuasarEnvironment.GetGame().GetChatManager().GetCommands().TryGetCommand(this.StringData.Replace(":", "").ToLower(), out ChatCommand))
                return false;

            if (Player.IChatCommand == ChatCommand)
            {
                Player.WiredInteraction = true;
                ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(this);
                ICollection<IWiredItem> Conditions = Instance.GetWired().GetConditions(this);

                foreach (IWiredItem Condition in Conditions.ToList())
                {
                    if (!Condition.Execute(Player))
                        return false;

                    Instance.GetWired().OnEvent(Condition.Item);
                }

                Player.GetClient().SendMessage(new WhisperComposer(User.VirtualId, this.StringData, 0, 0));
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
                    foreach (IWiredItem Effect in Effects.ToList())
                    {
                        if (!Effect.Execute(Player))
                            return false;

                        Instance.GetWired().OnEvent(Effect.Item);
                    }
                }
                return true;
            }

            return false;
        }
    }
}
