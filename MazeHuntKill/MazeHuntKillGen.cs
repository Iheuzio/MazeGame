using Maze;
using System.Diagnostics;

namespace MazeHuntKill
{
    public class HuntKillMazeGen : IMapProvider
    {
        private readonly int? _seed;
        private readonly Random _random;
        private int _width;
        private int _height;
        private Direction[,] _maze;

        public HuntKillMazeGen(int? seed = null)
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
                // Randomly select an unvisited cell for hunting
                int randomIndex = _random.Next(unvisitedCells.Count);
                MapVector startCell = unvisitedCells[randomIndex];
                unvisitedCells.RemoveAt(randomIndex);

                Walk(startCell, unvisitedCells);
            }

            return _maze;
        }

        public Direction[,] CreateMap()
        {
            // call the other CreateMap method with default values of 5 and 5
            return CreateMap(5, 5);
        }

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
    }
}
