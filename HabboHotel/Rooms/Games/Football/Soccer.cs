using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;

using Quasar.Communication.Packets.Incoming;

using Quasar.HabboHotel.Items;
using Quasar.HabboHotel.GameClients;
using Quasar.HabboHotel.Pathfinding;

using Quasar.Communication.Packets.Outgoing.Rooms.Engine;
using Quasar.HabboHotel.Rooms.Games.Teams;
using Quasar.HabboHotel.Items.Wired;
using Quasar.Football;
using System.Threading.Tasks;
using System.Threading;
using Quasar.HabboHotel.Rooms;

namespace Quasar.HabboHotel.Rooms.Games.Football
{
    public class Soccer
    {
        private Room _room;
        private Item[] gates;
        private ConcurrentDictionary<int, Item> _balls;
        private bool _gameStarted;

        public Soccer(Room room)
        {
            this._room = room;
            this.gates = new Item[4];
            this._balls = new ConcurrentDictionary<int, Item>();
            this._gameStarted = false;
        }
        public bool GameIsStarted
        {
            get { return this._gameStarted; }
        }
        public void StopGame(bool userTriggered = false)
        {
            this._gameStarted = false;

            if (!userTriggered)
                _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameEnds, null);
        }

        public void StartGame()
        {
            this._gameStarted = true;
            _room.GetWired().TriggerEvent(WiredBoxType.TriggerGameStarts, null);
        }

        public void AddBall(Item item)
        {
            this._balls.TryAdd(item.Id, item);
        }

        public void RemoveBall(int itemID)
        {
            Item Item = null;
            this._balls.TryRemove(itemID, out Item);
        }

        internal void OnUserWalk(RoomUser User)
        {
            try
            {

                if (User == null)
                    return;

                foreach (Item item in this._balls.Values.ToList())
                {
                    if (item == null)
                        continue;

                    if (User.SetX == item.GetX && User.SetY == item.GetY && User.GoalX == item.GetX && User.GoalY == item.GetY && User.handelingBallStatus == 0) // super chute.
                    {
                        Point userPoint = new Point(User.X, User.Y);
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;
                        MoveBall(item, User, userPoint, false, 6);
                    }

                    else if (User.X == item.GetX && User.Y == item.GetY && User.handelingBallStatus == 0)
                    {
                        Point userPoint = new Point(User.SetX, User.SetY);
                        item.ExtraData = "55";
                        item.BallIsMoving = true;
                        item.BallValue = 1;
                        MoveBall(item, User, userPoint, false, 6);
                    }
                    else
                    {
                        if (User.handelingBallStatus == 1 && User.GoalX == User.SetX && User.GoalY == User.SetY)
                            continue;

                        if (User.SetX != item.GetX || User.SetY != item.GetY || !User.IsWalking ||
                            (User.X == User.GoalX && User.Y == User.GoalY))
                            continue;

                        int num = User.X - item.GetX;
                        int num2 = User.Y - item.GetY;
                        int x = num * -1;
                        int y = num2 * -1;
                        x += item.GetX;
                        y += item.GetY;

                        User.handelingBallStatus = 1;
                        IComeDirection comeDirection = ComeDirection.GetComeDirection(new Point(User.X, User.Y), item.Coordinate, false, null);
                        if (comeDirection == IComeDirection.Null)
                            continue;

                        int newX = User.X;
                        int newY = User.Y;

                        if (item.GetRoom().GetGameMap().SquareHasUsers(x, y) || !item.GetRoom().GetGameMap().StackTable(x, y))
                        {
                            comeDirection = ComeDirection.InverseDirections(_room, comeDirection, User.X, User.Y);
                            newX = item.GetX;
                            newY = item.GetY;
                        }

                        ComeDirection.GetNewCoords(comeDirection, ref newX, ref newY);
                        item.ExtraData = "11";
                        MoveBall(item, User.GetClient(), x, y, true);
                    }
                }
            }
            catch
            {
            }
        }

        private bool VerifyBall(RoomUser user, int actualx, int actualy)
        {
            return Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;
        }

        public void RegisterGate(Item item)
        {
            if (gates[0] == null)
            {
                item.team = TEAM.BLUE;
                gates[0] = item;
            }
            else if (gates[1] == null)
            {
                item.team = TEAM.RED;
                gates[1] = item;
            }
            else if (gates[2] == null)
            {
                item.team = TEAM.GREEN;
                gates[2] = item;
            }
            else if (gates[3] == null)
            {
                item.team = TEAM.YELLOW;
                gates[3] = item;
            }
        }

        public void UnRegisterGate(Item item)
        {
            switch (item.team)
            {
                case TEAM.BLUE:
                    {
                        gates[0] = null;
                        break;
                    }
                case TEAM.RED:
                    {
                        gates[1] = null;
                        break;
                    }
                case TEAM.GREEN:
                    {
                        gates[2] = null;
                        break;
                    }
                case TEAM.YELLOW:
                    {
                        gates[3] = null;
                        break;
                    }
            }
        }

        public void onGateRemove(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FOOTBALL_GOAL_RED:
                case InteractionType.footballcounterred:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.RED);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_GREEN:
                case InteractionType.footballcountergreen:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.GREEN);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_BLUE:
                case InteractionType.footballcounterblue:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.BLUE);
                        break;
                    }
                case InteractionType.FOOTBALL_GOAL_YELLOW:
                case InteractionType.footballcounteryellow:
                    {
                        _room.GetGameManager().RemoveFurnitureFromTeam(item, TEAM.YELLOW);
                        break;
                    }
            }
        }

        private IEnumerable<Item> GetFootballItemsForAllTeams()
        {
            List<Item> items = _room.GetGameManager().GetFurniItems(TEAM.RED).Values.ToList();
            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.GREEN).Values);

            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.BLUE).Values);

            items.AddRange(_room.GetGameManager().GetFurniItems(TEAM.YELLOW).Values);

            return items;
        }

        private bool GameItemOverlaps(Item gameItem)
        {
            Point gameItemCoord = gameItem.Coordinate;
            return
                GetFootballItemsForAllTeams()
                    .Any(
                        item =>
                            item.GetAffectedTiles.Values.Any(
                                tile => tile.X == gameItemCoord.X && tile.Y == gameItemCoord.Y));
        }

        public bool MoveBall(Item item, GameClient mover, int newX, int newY, bool bNew)
        {
            if (item == null || item.GetBaseItem() == null)
                return false;


            bool itemIsOnGameItem = GameItemOverlaps(item);

            if (!_room.GetGameMap().itemCanBePlacedHere(newX, newY))
                return false;

            Point oldRoomCoord = item.Coordinate;
            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
                return false;

            double NewZ = _room.GetGameMap().Model.SqFloorHeight[newX, newY];
            if (bNew)
                this._room.SendMessage(new UpdateFootBallComposer(item, newX, newY, NewZ));
            else
                this._room.SendMessage(new SlideObjectBundleComposer(item.Coordinate.X, item.Coordinate.Y, item.GetZ, newX, newY, NewZ, item.Id, item.Id, item.Id));

            this._room.GetRoomItemHandler().SetFloorItem(null, item, newX, newY, item.Rotation, false, false, false, false);

            if (itemIsOnGameItem || mover == null || mover.GetHabbo() == null)
                return false;

            this._room.OnUserShoot(mover, item);

            return false;
        }

        public void MoveBall(Item item, RoomUser Player, Point user, bool Shift, int Shots)
        {
            try
            {
                item.comeDirection = ComeDirection.GetComeDirection(user, item.Coordinate, Shift, Shift == true ? Player : null);

                if (item.comeDirection != IComeDirection.Null)
                    new TaskFactory().StartNew(() => MoveBallProcess(item, Player.GetClient(), Shots));
            }
            catch
            {
            }
        }

        public async void MoveBallProcess(Item Item, GameClient client, int Shots)
        {
            int tryes = 0;
            int newX = Item.Coordinate.X;
            int newY = Item.Coordinate.Y;
            Item.interactingBallUser = 1;

            {
                if (this._room == null || this._room.GetGameMap() == null)
                    return;

                int total = Item.ExtraData == "55" ? Shots : 1;
                for (var i = 0; i != total; i++)
                {
                    if (Item.comeDirection == IComeDirection.Null)
                    {
                        Item.BallIsMoving = false;
                        Item.interactingBallUser = 0;
                        break;
                    }

                    int resetX = newX;
                    int resetY = newY;
                    ComeDirection.GetNewCoords(Item.comeDirection, ref newX, ref newY);

                    if (!this._room.GetGameMap().StackTable(newX, newY) || this._room.GetGameMap().SquareHasUsers(newX, newY))
                    {
                        Item.comeDirection = ComeDirection.InverseDirections(this._room, Item.comeDirection, newX, newY);
                        newX = resetX;
                        newY = resetY;
                        tryes++;
                        if (tryes > 2)
                        {
                            Item.BallIsMoving = false;
                            Item.interactingBallUser = 0;
                        }
                        continue;
                    }

                    if (MoveBall(Item, client, newX, newY, false))
                    {
                        Item.BallIsMoving = false;
                        Item.interactingBallUser = 0;
                        break;
                    }

                    int number;
                    int.TryParse(Item.ExtraData, out number);
                    if (number > 11)
                        Item.ExtraData = (int.Parse(Item.ExtraData) - 11).ToString();

                    await Task.Delay(90);
                }
                Item.interactingBallUser = 0;
                Item.BallValue++;

                if (Item.BallValue <= Shots)
                    return;

                Item.BallIsMoving = false;
                Item.BallValue = 1;
            }
        }

        public void Dispose()
        {
            Array.Clear(gates, 0, gates.Length);
            gates = null;
            _room = null;
            _balls.Clear();
            _balls = null;
        }
    }
}
