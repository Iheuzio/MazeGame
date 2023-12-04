using System;

namespace Maze
{
    public class MapVector : IMapVector
    {
        public int X { get; }
        public int Y { get; }

        public MapVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsValid => X >= 0 && Y >= 0;

        public bool InsideBoundary(int width, int height)
        {
            return X >= 0 && X < width && Y >= 0 && Y < height;
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public static MapVector operator +(MapVector v1, MapVector v2)
        {
            return new MapVector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static MapVector operator -(MapVector v1, MapVector v2)
        {
            return new MapVector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static MapVector operator *(MapVector vector, int scalar)
        {
            return new MapVector(vector.X * scalar, vector.Y * scalar);
        }

        public static implicit operator MapVector(Direction direction)
        {
            int x = 0, y = 0;

            switch (direction)
            {
                case Direction.N:
                    y--;
                    break;
                case Direction.S:
                    y++;
                    break;
                case Direction.W:
                    x--;
                    break;
                case Direction.E:
                    x++;
                    break;
                default:
                    throw new ArgumentException("Invalid direction passsed into map vector");
            }
            return new MapVector(x, y);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is MapVector other)
            {
                return X == other.X && Y == other.Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
