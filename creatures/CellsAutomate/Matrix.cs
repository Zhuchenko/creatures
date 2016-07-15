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
    public class Matrix
    {
        public int Length;
        public int Width;

        public Creature[,] Cells { get; set; }

        public Matrix(int n, int m)
        {
            Length = n;
            Width = m;
            Eat = new int[n, m];
            for (int i = 0; i < Length; i++)
                for (int j = 0; j < Width; j++)
                    Eat[i, j] = 0;
        }

        public IEnumerable<Creature> CellsAsEnumerable
        {
            get
            {
                for (int i = 0; i < Length; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if (Cells[i, j] != null)
                            yield return Cells[i, j];
                    }
                }
            }
        }

        public int[,] Eat { get; private set; }

        public int AliveCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Length; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if (Cells[i, j] != null) count++;
                    }
                }

                return count;
            }
        }

        public void CanBeReached()
        {
            var placeHoldersMatrix = new bool[Length, Width];

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    placeHoldersMatrix[i, j] = Cells[i, j] != null;
                }
            }

            var reachMatrixBuilder = new ReachMatrixBuilder();

            Eat = reachMatrixBuilder.Build(placeHoldersMatrix, Eat, 0, 0);
            Eat = reachMatrixBuilder.Build(placeHoldersMatrix, Eat, 0, Width - 1);
            Eat = reachMatrixBuilder.Build(placeHoldersMatrix, Eat, Length - 1, 0);
            Eat = reachMatrixBuilder.Build(placeHoldersMatrix, Eat, Length - 1, Width - 1);
        }

        public void FillStartMatrixRandomly()
        {
            var random = new Random();
            //var i = random.Next(N);
            //var j = random.Next(M);

            var executor = new Executor();
            var commands = new SeedGenerator().StartAlgorithm;


            //Cells[20, 20] = new Creature(new Point(20, 20), executor, commands, random);

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Cells[i, j] = random.Next(100) % 4 == 0 ? new Creature(new Point(i, j), executor, commands, random, 1) : null;
                }
            }

            CanBeReached();
        }

        public string PrintStartMatrix()
        {
            var result = new bool[Length, Width];

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    result[i, j] = Cells[i, j] != null;
                }
            }

            return PrintMatrix(result);
        }

        private string PrintMatrix(bool[,] matrix)
        {
            var result = new StringBuilder();

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    result.Append(matrix[i, j] ? "*" : " ");
                }
                result.AppendLine("|");
            }

            return result.ToString();
        }

        public void MakeTurn()
        {
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    MakeTurn(Cells[i, j], Eat, i, j);
                }
            }

            CanBeReached();
        }

        private void MakeTurn(Creature simpleCreature, int[,] eat, int i, int j)
        {
            if (simpleCreature == null) return;

            var resultTuple = simpleCreature.MyTurn(eat, Cells);

            var result = resultTuple.Item2;

            if (resultTuple.Item1)
            {
                var newPosition = ActionEx.PointByAction(result, new Point(i, j), Cells);

                Cells[newPosition.X, newPosition.Y] = simpleCreature.MakeChild(newPosition);

                return;
            }

            if (result == ActionEnum.Stay) return;
            if (result == ActionEnum.Die) Cells[i, j] = null;
            else
            {
                var newPosition = ActionEx.PointByAction(result, new Point(i, j), Cells);
                simpleCreature.SetPosition(newPosition);
                Cells[i, j] = null;
                Cells[newPosition.X, newPosition.Y] = simpleCreature;
            }
        }
    }
}
