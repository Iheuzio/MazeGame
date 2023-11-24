using Maze;

namespace MazeHuntKill;
public class MapVector
{
    public int X { get; set; }
    public int Y { get; set; }

    public MapVector(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Move in the specified direction and return the new position as a MapVector for hunt and kill algorithm
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public MapVector Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.N:
                return new MapVector(X, Y - 1);
            case Direction.S:
                return new MapVector(X, Y + 1);
            case Direction.E:
                return new MapVector(X + 1, Y);
            case Direction.W:
                return new MapVector(X - 1, Y);
            default:
                return this;
        }
    }
}
