using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.Items.Utilities;

namespace Quasar.HabboHotel.Catalog.Utilities
{
    public static class ItemUtility
    {
        public static bool CanGiftItem(CatalogItem Item)
        {
            if (!Item.Data.AllowGift || Item.IsLimited || Item.Amount > 1 || Item.Data.ItemName.ToLower().StartsWith("cf_") || Item.Data.ItemName.ToLower().StartsWith("cfc_") ||
                Item.Data.InteractionType == InteractionType.BADGE || (Item.Data.Type != 's' && Item.Data.Type != 'i') || Item.CostDiamonds > 0 ||
                Item.Data.InteractionType == InteractionType.TELEPORT || Item.Data.InteractionType == InteractionType.DEAL)
                return false;

            if (PetUtility.IsPet(Item.Data.InteractionType))
                return false;
            return true;
        }

        public static bool CanSelectAmount(CatalogItem Item)
        {
            if (Item.IsLimited || Item.Amount > 1 || Item.Data.ItemName.ToLower().StartsWith("cf_") || Item.Data.ItemName.ToLower().StartsWith("cfc_") || !Item.HaveOffer || Item.Data.InteractionType == InteractionType.BADGE || Item.Data.InteractionType == InteractionType.DEAL)
                return false;
            return true;
        }

        public static int GetSaddleId(int Saddle)
        {
            switch (Saddle)
            {
                default:
                case 9:
                    return QuasarEnvironment.GetGame().GetItemManager().GetItemByName("horse_saddle1").Id;
                case 10:
                    return QuasarEnvironment.GetGame().GetItemManager().GetItemByName("horse_saddle2").Id;
            }
        }

        public static bool IsRare(Item Item)
        {
            if (Item.LimitedNo > 0)
                return true;

            if (Item.Data.IsRare)
                return true;

            return false;
        }
    }
}