using Maze;
using System;
using System.Collections.Generic;

namespace MazeRecursion
{
    public class RecursiveMazeGen : IMapProvider
    {
        private readonly int? _seed;
        private readonly Random _random;
        private int _width;
        private int _height;

        public RecursiveMazeGen(int? seed = null)
        {
            _seed = seed;
            _random = (_seed == null)
                ? new Random()
                : new Random((int)_seed);
        }

        public Direction[,] CreateMap(int width, int height)
        {
            if (width % 2 == 0 || height % 2 == 0)
            {
                throw new ArgumentException("Width and height must be odd.");
            }

            _width = width / 2;
            _height = height / 2;

            Direction[,] maze = new Direction[_height, _width];
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    maze[i, j] = Direction.None;
                }
            }

            GenerateMaze(maze, 0, 0);

            return maze;
        }

        public Direction[,] CreateMap()
        {
            // call the other CreateMap method with default values of 5 and 5
            return CreateMap(5, 5);
        }

        private void GenerateMaze(Direction[,] maze, int x, int y)
        {
            List<Direction> directions = new List<Direction> { Direction.N, Direction.S, Direction.E, Direction.W };
            Shuffle(directions);

            foreach (Direction direction in directions)
            {
                int newX = x;
                int newY = y;

                switch (direction)
                {
                    case Direction.N:
                        newY--;
                        break;
                    case Direction.S:
                        newY++;
                        break;
                    case Direction.E:
                        newX++;
                        break;
                    case Direction.W:
                        newX--;
                        break;
                }

                if (IsInBounds(newX, newY, maze) && maze[newY, newX] == Direction.None)
                {
                    maze[y, x] |= direction;
                    maze[newY, newX] |= GetOppositeDirection(direction);
                    GenerateMaze(maze, newX, newY);
                }
            }
        }

        private Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.N:
                    return Direction.S;
                case Direction.S:
                    return Direction.N;
                case Direction.E:
                    return Direction.W;
                case Direction.W:
                    return Direction.E;
                default:
                    return Direction.None;
            }
        }

        private bool IsInBounds(int x, int y, Direction[,] maze)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        private void Shuffle(List<Direction> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1);
                Direction value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
