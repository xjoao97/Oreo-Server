using System;
using System.Linq;
using System.Collections.Generic;

using Quasar.HabboHotel.Items;
using Quasar.Communication.Packets.Outgoing.Inventory.Furni;

using Quasar.Communication.Packets.Outgoing.Rooms.Furni;
using Quasar.HabboHotel.Items.Crafting;

namespace Quasar.Communication.Packets.Incoming.Rooms.Furni
{
    class GetCraftingItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            /*var result = Packet.PopString();

            CraftingRecipe recipe = null;
            foreach (CraftingRecipe Receta in QuasarEnvironment.GetGame().GetCraftingManager().CraftingRecipes.Values)
            {
                if (Receta.Result.Contains(result))
                {
                    recipe = Receta;
                    break;
                }
            }

            var Final = QuasarEnvironment.GetGame().GetCraftingManager().GetRecipe(recipe.Id);

            Session.SendMessage(new CraftingResultComposer(recipe, true));
            Session.SendMessage(new CraftableProductsComposer());*/
        }
    }
}
