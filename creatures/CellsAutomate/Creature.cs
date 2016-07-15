using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Creatures.Language.Commands.Interfaces;
using Creatures.Language.Executors;

namespace CellsAutomate
{
    public class Creature
    {
        private readonly Random _random;
        private Point _position;
        private readonly Executor _executor;
        private readonly ICommand[] _commands;

        public static int N = 1;  // Food level
        private int K = 3;  // Minimum to survive
        private int C = 6;  // Child price
        private int M = 3; // Food for 1 time
        private int T = 20; //Maximum turns

        private int _turns;
        private int _store = 1;

        public int Generation { get; private set; }

        public Creature(Point position, Executor executor, ICommand[] commands, Random random, int generation)
        {
            _position = position;
            _executor = executor;
            _commands = commands;
            _random = random;
            Generation = generation;
        }

        public Tuple<bool, ActionEnum> MyTurn(int[,] eatMatrix, Creature[,] cellsMatrix)
        {
            if (_store < K || _turns >= T)
                return Tuple.Create(false, ActionEnum.Die);

            _store -= K;
            _turns++;

            if (_store >= C)
            {
                var directions = new List<ActionEnum>();
                var points = ActionEx.GetPoints(_position.X, _position.Y);
                foreach (var item in points)
                {
                    if (isValid(item, eatMatrix) && cellsMatrix[item.X, item.Y] == null)
                    {
                        directions.Add(ActionEx.ActionByPoint(_position, item));
                    }
                }

                if (directions.Count != 0)
                {
                    _store -= C;
                    return Tuple.Create(true, directions.ElementAt(0));
                }
            }

            var state =
                ActionEx
                    .GetPoints(_position.X, _position.Y)
                    .ToDictionary(x => ActionEx.DirectionByPoint(_position, x), x => (isValid(x, eatMatrix) && eatMatrix[x.X, x.Y] != 0) ? 4 : 0);

            var result = _executor.Execute(_commands, new MyExecutorToolset(_random, state));
            var parsedResult = int.Parse(result);


            ActionEnum action;
            switch (parsedResult)
            {
                case 0:
                    action = ActionEnum.Stay;
                    if (eatMatrix[_position.X, _position.Y] >= M)
                    {
                        _store += M;
                        eatMatrix[_position.X, _position.Y] -= M;
                    }
                    break;
                case 1: action = ActionEnum.Up; Stats.Up++; break;
                case 2: action = ActionEnum.Right; Stats.Right++; break;
                case 3: action = ActionEnum.Down; Stats.Down++; break;
                case 4: action = ActionEnum.Left; Stats.Left++; break;
                default: throw new Exception();
            }

            return Tuple.Create(false, action);
        }

        private bool isValid(Point x, int[,] eatMatrix)
        {
            if (x.X < 0) return false;
            if (x.Y < 0) return false;
            if (x.X >= eatMatrix.GetLength(0)) return false;
            if (x.Y >= eatMatrix.GetLength(1)) return false;

            return true;
        }

        public Creature MakeChild(Point position)
        {
            return new Creature(position, _executor, _commands, _random, this.Generation + 1);
        }

        internal void SetPosition(Point newPosition)
        {
            _position = newPosition;
        }
    }
}
