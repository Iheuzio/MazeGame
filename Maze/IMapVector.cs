namespace Maze
{

    public interface IMapVector
    {
        /// <summary>
        /// Determines if the position is legal
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// The X position of the vector
        /// </summary>
        int X { get; }
        /// <summary>
        /// The Y position of the vector
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Determines if the vector point is inside the map
        /// </summary>
        /// <param name="width">The width of the map</param>
        /// <param name="height">The height of the map</param>
        /// <returns>True if the vector is inside the map limits</returns>
        bool InsideBoundary(int width, int height);
        /// <summary>
        /// Computes the euclidean magnitude of the vector
        /// </summary>
        /// <returns>Returns the magnitude of the vector</returns>
        double Magnitude();
    }
}