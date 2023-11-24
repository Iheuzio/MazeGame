using System;

namespace Maze
{
    public class Map : IMap
    {
        private readonly IMapProvider _mapProvider;

        public Map(IMapProvider mapProvider)
        {
            _mapProvider = mapProvider ?? throw new ArgumentNullException("Invalid IMapProvider provided");
        }

        public MapVector Goal { get; private set; }
        public int Height { get; private set; }
        public bool IsGameFinished { get; private set; }
        public Block[,] MapGrid { get; private set; }
        public IPlayer Player { get; private set; }
        public int Width { get; private set; }

        public Direction[,]? directionMap = null;

        public void CreateMap()
        {
            if(directionMap == null)
                directionMap = _mapProvider.CreateMap();
            Width = directionMap.GetLength(1) * 2 + 1;
            Height = directionMap.GetLength(0) * 2 + 1;
            if ((Height % 2) == 0 || (Width % 2) == 0)
            {
                throw new ArgumentException("Height and width must be odd");
            }
            MapGrid = new Block[Width, Height];

            // Initialize the map with solid blocks
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    MapGrid[x, y] = Block.Solid;
                }
            }

            // Create the maze based on the direction map
            for (int y = 0; y < directionMap.GetLength(0); y++)
            {
                for (int x = 0; x < directionMap.GetLength(1); x++)
                {
                    int gridX = x * 2 + 1;
                    int gridY = y * 2 + 1;

                    MapGrid[gridX, gridY] = Block.Empty;

                    if (directionMap[y, x].HasFlag(Direction.N))
                    {
                        MapGrid[gridX, gridY - 1] = Block.Empty;
                    }
                    else if (directionMap[y, x].HasFlag(Direction.S))
                    {
                        MapGrid[gridX, gridY + 1] = Block.Empty;
                    }

                    if (directionMap[y, x].HasFlag(Direction.E))
                    {
                        MapGrid[gridX + 1, gridY] = Block.Empty;
                    }

                    if (directionMap[y, x].HasFlag(Direction.W))
                    {
                        MapGrid[gridX - 1, gridY] = Block.Empty;
                    }
                }
            }

            // Set emptyblocks
            Random random = new Random();
            List<MapVector> emptyBlocks = new List<MapVector>();

            for (int y = 1; y < Height - 1; y++)
            {
                for (int x = 1; x < Width - 1; x++)
                {
                    if (MapGrid[x, y] == Block.Empty)
                    {
                        emptyBlocks.Add(new MapVector(x, y));
                    }
                }
            }

            // check if teh map has all empty blocks connected
            if (!IsMapValid(emptyBlocks))
            {
                throw new Exception("Map is not valid");
            }

            int randomIndex = random.Next(emptyBlocks.Count);
            Player = new Player(emptyBlocks[randomIndex], Direction.N, MapGrid);

            Goal = FindDeadEnd(emptyBlocks, Player.Position);
            IsGameFinished = false;
        }

        private bool IsMapValid(List<MapVector> emptyBlocks)
        {
            if (emptyBlocks.Count == 0)
            {
                // If there are no empty blocks, the map is invalid
                return false;
            }

            List<MapVector> visited = new List<MapVector>();
            List<MapVector> blockVisited = new List<MapVector>();

            // Start with the first empty block
            MapVector startBlock = emptyBlocks[0];
            visited.Add(startBlock);
            blockVisited.Add(startBlock);

            while (blockVisited.Count > 0)
            {
                MapVector currentBlock = blockVisited[blockVisited.Count - 1];
                bool foundUnvisitedNeighbor = false;

                foreach (MapVector neighbor in GetAdjacentEmptyBlocks(currentBlock, emptyBlocks))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        blockVisited.Add(neighbor);
                        foundUnvisitedNeighbor = true;
                        break; // Exit the loop after finding an unvisited neighbor
                    }
                }

                if (!foundUnvisitedNeighbor)
                {
                    // If all neighbors have been visited, backtrack
                    blockVisited.RemoveAt(blockVisited.Count - 1);
                }
            }

            // If all empty blocks have been visited, the map is valid
            return visited.Count == emptyBlocks.Count;
        }

        private List<MapVector> GetAdjacentEmptyBlocks(MapVector position, List<MapVector> emptyBlocks)
        {
            List<MapVector> neighbors = new List<MapVector>();

            int x = position.X;
            int y = position.Y;

            foreach (MapVector neighbor in emptyBlocks)
            {
                int newX = neighbor.X;
                int newY = neighbor.Y;

                // if the neighbor shares an x or a y axis with the current position, add it to the list of adjacent blocks
                if ((Math.Abs(newX - x) == 1 && newY == y) || Math.Abs(newY - y) == 1 && newX == x)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private MapVector FindDeadEnd(List<MapVector> emptyBlocks, MapVector playerPosition)
        {
            MapVector? furthestDeadEnds = null;
            // just a value that will be overwritten if a valid deadend is found
            int maxDistance = -1;

            foreach (MapVector position in emptyBlocks)
            {
                if (IsDeadEnd(position))
                {
                    int distance = CalculateDistance(playerPosition, position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        // add new distance since its further
                        furthestDeadEnds = position;
                    }
                    // two deadends are the same distance away
                    else if (distance == maxDistance)
                    {
                        return position;
                    }
                }
            }

            if (furthestDeadEnds != null)
            {
                return furthestDeadEnds;
            }

            // should only execute if no valid deadends were found
            throw new Exception("No valid deadends found");
        }


        private bool IsDeadEnd(MapVector position)
        {
            int countEmptyNeighbors = 0;

            int x = position.X;
            int y = position.Y;

            if (MapGrid[x, y - 1] == Block.Empty) countEmptyNeighbors++;
            if (MapGrid[x, y + 1] == Block.Empty) countEmptyNeighbors++;
            if (MapGrid[x - 1, y] == Block.Empty) countEmptyNeighbors++;
            if (MapGrid[x + 1, y] == Block.Empty) countEmptyNeighbors++;

            return countEmptyNeighbors == 1; // A dead end has only one empty neighbor
        }

        public void FoundEndGoal() => IsGameFinished = true;

        private int CalculateDistance(MapVector from, MapVector to)
        {
            // distance formula thing (x1 - x2) + (y1 - y2)
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        public void CreateMap(int width, int height)
        {
            directionMap = _mapProvider.CreateMap(width, height);

            // no need to rewrite existing code
            CreateMap();

            // save the map?
            // will implement later
            //string fileName = $"map{width}x{height}.txt";
            //string mapDir = Path.Combine(Directory.GetCurrentDirectory(), "Maps");
            //fileName = Path.Combine(mapDir, fileName);
            //SaveDirectionMap(fileName);

        }

        public void SaveDirectionMap(string path)
        {
            // if file already exists, overwrite it, otherwise save it
        }
    }
}
