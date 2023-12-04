using Maze;
using System.Diagnostics;

namespace MazeHuntKill
{
    public class ImprovedMazeHuntKillGen : IMapProvider
    {
        private readonly int? _seed;
        private readonly Random _random;
        private int _width;
        private int _height;
        private Direction[,] _maze;

        public ImprovedMazeHuntKillGen(int? seed = null)
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

            List<MapVector> unvisitedCells = new List<MapVector>();
            MapVector current = new MapVector(_random.Next(_width), _random.Next(_height));
            Walk(current, unvisitedCells);

            while (unvisitedCells.Count > 0)
            {
                // Select the last unvisited cell for hunting
                MapVector startCell = unvisitedCells[unvisitedCells.Count - 1];
                unvisitedCells.RemoveAt(unvisitedCells.Count - 1);

                Walk(startCell, unvisitedCells);
            }

            return _maze;
        }

        /// <summary>
        /// Walk the maze from the specified cell until we hit a dead end or a visited cell with no unvisited neighbors 
        /// then call Hunt to find a new cell to start walking from.
        /// </summary>
        /// <param name="current"></param>
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
        /// <param name="unvisitedCells"></param>
        private void Walk(MapVector current, List<MapVector> unvisitedCells)
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
                unvisitedCells.Add(current);
            }
        }

        /// <summary>
        /// Hunt for a new cell to start walking from.
        /// </summary>
        /// <param name="unvisitedCells"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the opposite direction of the specified direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private static Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.N => Direction.S,
                Direction.S => Direction.N,
                Direction.E => Direction.W,
                Direction.W => Direction.E,
                _ => Direction.None,
            };
        }
    }
}
