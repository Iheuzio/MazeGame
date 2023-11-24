namespace Maze
{
    public enum Block { Solid, Empty }

    /// <summary>
    /// Represents a map of a maze
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// The Goal or exit of the maze
        /// </summary>
        MapVector Goal { get; }
        /// <summary>
        /// The height of the Maze in terms of Blocks
        /// </summary>
        int Height { get; }
        /// <summary>
        /// A boolean indicating the player has found the goal
        /// </summary>
        bool IsGameFinished { get; }
        /// <summary>
        /// The map grid representing the maze
        /// </summary>
        Block[,] MapGrid { get; }
        /// <summary>
        /// The player in the maze
        /// </summary>
        IPlayer Player { get; }
        /// <summary>
        /// The width of the Maze in terms of Blocks
        /// </summary>
        int Width { get; }
        /// <summary>
        /// Populates the MapGrid, Goal, and Player
        /// </summary>
        void CreateMap();
        /// <summary>
        /// Populates the MapGrid using the provided width and height, Goal, and Player
        /// </summary>
        /// <param name="width">The width of the maze in blocks</param>
        /// <param name="height">The height of the maze in blocks</param>
        void CreateMap(int width, int height);

        void SaveDirectionMap(string path);
    }
}