using Quasar.HabboHotel.Rooms;
using Quasar.HabboHotel.Rooms;
using Quasar.Football;
using System.Drawing;

namespace Quasar.Football
{
    public class ComeDirection
    {
        internal static IComeDirection GetComeDirection(Point user, Point ball, bool Click, RoomUser Player)
        {
            try
            {
                if (!Click)
                {
                    if (user.X == ball.X && user.Y - 1 == ball.Y)
                        return IComeDirection.Down;
                    if (user.X + 1 == ball.X && user.Y - 1 == ball.Y)
                        return IComeDirection.DownLeft;
                    if (user.X + 1 == ball.X && user.Y == ball.Y)
                        return IComeDirection.Left;
                    if (user.X + 1 == ball.X && user.Y + 1 == ball.Y)
                        return IComeDirection.UpLeft;
                    if (user.X == ball.X && user.Y + 1 == ball.Y)
                        return IComeDirection.Up;
                    if (user.X - 1 == ball.X && user.Y + 1 == ball.Y)
                        return IComeDirection.UpRight;
                    if (user.X - 1 == ball.X && user.Y == ball.Y)
                        return IComeDirection.Right;
                    if (user.X - 1 == ball.X && user.Y - 1 == ball.Y)
                        return IComeDirection.DownRight;
                }
                else if (Player != null)
                    {
                        //User Arriba - Pelota Abajo
                        if (Player.RotBody == 4)
                            return IComeDirection.Up;
                        //User Abajo - Pelota Arriba
                        if (Player.RotBody == 0)
                            return IComeDirection.Down;
                        //User Derecha - Pelota Izquierda
                        if (Player.RotBody == 6)
                            return IComeDirection.Right;
                        //User Izquierda - Pelota Derecha
                        if (Player.RotBody == 2)
                            return IComeDirection.Left;

                        //Diagonales
                        //User Abajo Derecha - Pelota Arriba Izquierda
                        if (Player.RotBody == 1)
                            return IComeDirection.DownLeft;
                        //User Abajo Izquierda - Pelota Arriba Derecha
                        if (Player.RotBody == 7)
                            return IComeDirection.DownRight;
                        //User Arriba Izquierda - Pelota Abajo Derecha
                        if (Player.RotBody == 3)
                            return IComeDirection.UpLeft;
                        //User Arriba Derecha - Pelota Abajo Izquierda
                        if (Player.RotBody == 5)
                            return IComeDirection.UpRight;
                    }
                
                return IComeDirection.Null;
            }
            catch
            {
                return IComeDirection.Null;
            }
        }

        internal static IComeDirection GetInverseDirectionEasy(IComeDirection comeWith)
        {
            IComeDirection result;
            try
            {
                switch (comeWith)
                {
                    case IComeDirection.Up:
                        result = IComeDirection.Down;
                        break;

                    case IComeDirection.UpRight:
                        result = IComeDirection.DownLeft;
                        break;

                    case IComeDirection.Right:
                        result = IComeDirection.Left;
                        break;

                    case IComeDirection.DownRight:
                        result = IComeDirection.UpLeft;
                        break;

                    case IComeDirection.Down:
                        result = IComeDirection.Up;
                        break;

                    case IComeDirection.DownLeft:
                        result = IComeDirection.UpRight;
                        break;

                    case IComeDirection.Left:
                        result = IComeDirection.Right;
                        break;

                    default:
                        result = comeWith == IComeDirection.UpLeft ? IComeDirection.DownRight : IComeDirection.Null;
                        break;
                }
            }
            catch
            {
                result = IComeDirection.Null;
            }
            return result;
        }

        internal static void GetNewCoords(IComeDirection comeWith, ref int newX, ref int newY)
        {
            try
            {
                switch (comeWith)
                {
                    case IComeDirection.Up:
                        newY++;
                        break;

                    case IComeDirection.UpRight:
                        newX--;
                        newY++;
                        break;

                    case IComeDirection.Right:
                        newX--;
                        break;

                    case IComeDirection.DownRight:
                        newX--;
                        newY--;
                        break;

                    case IComeDirection.Down:
                        newY--;
                        break;

                    case IComeDirection.DownLeft:
                        newX++;
                        newY--;
                        break;

                    case IComeDirection.Left:
                        newX++;
                        break;

                    case IComeDirection.UpLeft:
                        newX++;
                        newY++;
                        break;
                }
            }
            catch
            {
            }
        }

        internal static IComeDirection InverseDirections(Room room, IComeDirection comeWith, int x, int y)
        {
            try
            {
                switch (comeWith)
                {
                    case IComeDirection.Up:
                        return IComeDirection.Down;

                    case IComeDirection.UpRight:
                        if (room.GetGameMap().StaticModel.SqState[x, y] == SquareState.BLOCKED)
                            return room.GetGameMap().StaticModel.SqState[x + 1, y] == SquareState.BLOCKED
                                ? IComeDirection.DownRight
                                : IComeDirection.UpLeft;
                        return IComeDirection.DownRight;

                    case IComeDirection.Right:
                        return IComeDirection.Left;

                    case IComeDirection.DownRight:
                        if (room.GetGameMap().StaticModel.SqState[x, y] == SquareState.BLOCKED)
                            return room.GetGameMap().StaticModel.SqState[x + 1, y] == SquareState.BLOCKED
                                ? IComeDirection.UpRight
                                : IComeDirection.DownLeft;
                        return IComeDirection.UpRight;

                    case IComeDirection.Down:
                        return IComeDirection.Up;

                    case IComeDirection.DownLeft:
                        return IComeDirection.DownRight;

                    case IComeDirection.Left:
                        return IComeDirection.Right;

                    case IComeDirection.UpLeft:
                        return IComeDirection.UpRight;
                }
                return IComeDirection.Null;
            }
            catch
            {
                return IComeDirection.Null;
            }
        }
    }
}
