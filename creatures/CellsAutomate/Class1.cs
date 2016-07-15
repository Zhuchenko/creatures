using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Creatures.Language.Commands.Interfaces;
using Creatures.Language.Parsers;
using Creatures.Language.Executors;

namespace CellsAutomate
{
    public enum ActionEnum
    {
        Die,
        Left,
        Right,
        Up,
        Down,
        Stay
    }

    static class ActionEx
    {
        public static Point PointByAction(ActionEnum actionEnum, Point start, Creature[,] cellsMatrix)
        {
            switch (actionEnum)
            {
                case ActionEnum.Die:
                {
                    cellsMatrix[start.X, start.Y] = null;
                    throw new CreatureIsDeadException(start);
                }
                case ActionEnum.Left:
                    return new Point(start.X - 1, start.Y);
                case ActionEnum.Right:
                    return new Point(start.X + 1, start.Y);
                case ActionEnum.Up:
                    return new Point(start.X, start.Y - 1);
                case ActionEnum.Down:
                    return new Point(start.X, start.Y + 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionEnum), actionEnum, null);
            }
        }

        public static ActionEnum ActionByPoint(Point start, Point finish)
        {
            var xOffset = finish.X - start.X;
            var yOffset = finish.Y - start.Y;

            if (xOffset == -1 && yOffset == 0) return ActionEnum.Left;
            if (xOffset == 1 && yOffset == 0) return ActionEnum.Right;
            if (xOffset == 0 && yOffset == 1) return ActionEnum.Down;
            if (xOffset == 0 && yOffset == -1) return ActionEnum.Up;

            throw new ArgumentException();
        }

        public static Point[] GetPoints(int i, int j)
        {
            return new[] { new Point(i + 1, j), new Point(i, j + 1), new Point(i - 1, j), new Point(i, j - 1) };
        }

        public static int DirectionByPoint(Point start, Point finish)
        {
            var xOffset = finish.X - start.X;
            var yOffset = finish.Y - start.Y;

            if (xOffset == 0 && yOffset == -1) return 0;
            if (xOffset == 1 && yOffset == 0) return 1;
            if (xOffset == 0 && yOffset == 1) return 2;
            if (xOffset == -1 && yOffset == 0) return 3;

            throw new ArgumentException();
        }
    }
}