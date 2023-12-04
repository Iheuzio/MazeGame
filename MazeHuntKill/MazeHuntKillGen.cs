﻿using Maze;
using System.Diagnostics;

namespace MazeHuntKill
{
    public class HuntKillMazeGen : IMapProvider
    {
        private readonly int? _seed;
        private readonly Random _random;
        private int _width;
        private int _height;

        // don't need to check for nullability since it's initialized in CreateMap method and never set to null, we populate it
        private Direction[,]? _maze;

        public HuntKillMazeGen(int? seed = null)
        {
            _seed = seed;
            _random = (_seed == null)
                ? new Random()
                : new Random((int)_seed);
        }

        /// <summary>
        /// Create a new maze with the specified width and height using the Hunt and Kill algorithm.
        /// Will run until it finds a cell with no unvisited neighbors, then call Hunt to find a new cell to start walking from.
        /// This is until the maze has all cells visited.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Direction[,] CreateMap(int width, int height)
        {
            if (width % 2 == 0 || height % 2 == 0)
            {
                throw new ArgumentException("Width and height must be odd.");
            }

            _width = width / 2;
            _height = height / 2;

            _maze = new Direction[_height, _width];
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _maze[i, j] = Direction.None;
                }
            }

            MapVector current = new MapVector(_random.Next(_width), _random.Next(_height));
            Walk(current);

            // while maze doesn't contain a None value, keep walking
           for(int i = 0; i < _height; i++)
            {
                for(int j = 0; j < _width; j++)
                {
                    if (_maze[i,j] == Direction.None)
                    {
                        Walk(new MapVector(j, i));
                    }
                }
            }

            return _maze;
        }

        public Direction[,] CreateMap()
        {
            // call the other CreateMap method with default values of 5 and 5
            return CreateMap(5, 5);
        }

        /// <summary>
        /// Walk the maze from the specified cell until we hit a dead end or a visited cell with no unvisited neighbors 
        /// then call Hunt to find a new cell to start walking from.
        /// </summary>
        /// <param name="current"></param>
        private void Walk(MapVector current)
        {
            Debug.Assert(_maze != null, "_maze != null");
            while (true)
            {
                List<Direction> directions = new List<Direction>();
                if (current.X > 0 && _maze[current.Y, current.X - 1] == Direction.None)
                {
                    directions.Add(Direction.W);
                }
                if (current.X < _width - 1 && _maze[current.Y, current.X + 1] == Direction.None)
                {
                    directions.Add(Direction.E);
                }
                if (current.Y > 0 && _maze[current.Y - 1, current.X] == Direction.None)
                {
                    directions.Add(Direction.N);
                }
                if (current.Y < _height - 1 && _maze[current.Y + 1, current.X] == Direction.None)
                {
                    directions.Add(Direction.S);
                }

                if (directions.Count == 0)
                {
                    break;
                }

                Shuffle(directions);

                Direction direction = directions[0];
                MapVector next = current.Move(direction);
                _maze[current.Y, current.X] |= direction;
                _maze[next.Y, next.X] |= GetOppositeDirection(direction);

                current = next;
            }

            // After walking, perform hunting if needed
            Hunt();
        }

        /// <summary>
        /// Hunt for a cell that has at least one visited neighbor
        /// </summary>
        private void Hunt()
        {
            Debug.Assert(_maze != null, "_maze != null");

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_maze[y, x] != Direction.None)
                    {
                        continue;
                    }

                    List<Direction> directions = new List<Direction>();
                    if (x > 0 && _maze[y, x - 1] != Direction.None)
                    {
                        directions.Add(Direction.W);
                    }
                    if (x < _width - 1 && _maze[y, x + 1] != Direction.None)
                    {
                        directions.Add(Direction.E);
                    }
                    if (y > 0 && _maze[y - 1, x] != Direction.None)
                    {
                        directions.Add(Direction.N);
                    }
                    if (y < _height - 1 && _maze[y + 1, x] != Direction.None)
                    {
                        directions.Add(Direction.S);
                    }

                    if (directions.Count == 0)
                    {
                        continue;
                    }

                    Shuffle(directions);
                    Direction direction = directions[0];
                    MapVector current = new MapVector(x, y);
                    MapVector next = current.Move(direction);
                    _maze[current.Y, current.X] |= direction;
                    _maze[next.Y, next.X] |= GetOppositeDirection(direction);
                    Walk(next);
                    return;
                }
            }
        }

        /// <summary>
        /// Get the opposite direction of the specified direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Shuffle the list of directions to randomize the order
        /// </summary>
        /// <param name="list"></param>
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
