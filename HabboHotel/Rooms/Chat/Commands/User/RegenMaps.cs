namespace Quasar.HabboHotel.Rooms.Chat.Commands.User
{
    class RegenMaps : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_regen_maps"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Limpar pisos do quarto"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Apenas o proprietário deste quarto pode usar esse comando", 1);
                return;
            }

            Room.GetGameMap().GenerateMaps();
            Session.SendWhisper("Você passou desinfetante no chão do quarto com sucesso", 1);
        }
    }
}