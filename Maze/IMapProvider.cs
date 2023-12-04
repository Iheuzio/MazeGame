namespace Maze
{
    [Flags]
    public enum Direction { None = 0, N = 1, E = 2, S = 4, W = 8 }

    public interface IMapProvider
    {
        /// <summary>
        /// Creates a direction map, where (0,0) is the upper left corner with Y being positive downwards in rows and X being positive rightwards in columns
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>A Direction array representing the maze</returns>
        Direction[,] CreateMap(int width, int height);

        /// <summary>
        /// Create a direction map, where (0,0) is the upper left corner with Y being positive downwards in rows and X being positive rightwards in columns.
        /// Returns a map with a default size.
        /// </summary>
        /// <returns>A Direction array representing the maze</returns>
        Direction[,] CreateMap();
    }
}