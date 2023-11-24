using System;

namespace Maze
{
    public class Player : IPlayer
    {
        public Direction Facing { get; private set; }
        public MapVector Position { get; private set; }
        public int StartX { get; }
        public int StartY { get; }
        private readonly Block[,] MapGrid;

        public Player(MapVector startPosition, Direction initialFacing, Block[,] mapGrid)
        {
            // check if parameters are null and throw exception if they are
            if (startPosition == null || mapGrid == null)
            {
                throw new ArgumentNullException("Invalid start position provided");
            }
            if (!startPosition.InsideBoundary(mapGrid.GetLength(0), mapGrid.GetLength(1)))
            {
                throw new ArgumentException("Start position is outside of map grid");
            }
            if (mapGrid[startPosition.X, startPosition.Y] == Block.Solid)
            {
                throw new ArgumentException("Start position is solid");
            }
            if (initialFacing == Direction.None)
            {
                throw new ArgumentException("Invalid initial facing provided");
            }
            Facing = initialFacing;
            Position = startPosition;
            StartX = startPosition.X;
            StartY = startPosition.Y;
            MapGrid = mapGrid;
        }

        public void MoveBackward()
        {
            MapVector moveVector = GetBackwardMoveVector();
            MapVector newPosition = Position + moveVector;

            if (IsValidMove(newPosition))
            {
                Position = newPosition;
                UpdateMapGrid();
            }
        }

        public void MoveForward()
        {
            MapVector moveVector = GetForwardMoveVector();
            MapVector newPosition = Position + moveVector;

            if (IsValidMove(newPosition))
            {
                Position = newPosition;
                UpdateMapGrid();
            }
        }

        public void TurnLeft()
        {
            switch (Facing)
            {
                case Direction.N:
                    Facing = Direction.W;
                    break;
                case Direction.E:
                    Facing = Direction.N;
                    break;
                case Direction.S:
                    Facing = Direction.E;
                    break;
                case Direction.W:
                    Facing = Direction.S;
                    break;
                default:
                    throw new Exception("Invalid direction");
            }
        }

        public void TurnRight()
        {
            switch (Facing)
            {
                case Direction.N:
                    Facing = Direction.E;
                    break;
                case Direction.E:
                    Facing = Direction.S;
                    break;
                case Direction.S:
                    Facing = Direction.W;
                    break;
                case Direction.W:
                    Facing = Direction.N;
                    break;
                default:
                    throw new Exception("Invalid direction");
            }
        }

        public float GetRotation()
        {
            switch (Facing)
            {
                case Direction.N:
                    return 0;
                case Direction.E:
                    return (1.0f / 2.0f * (float)Math.PI);
                case Direction.S:
                    return (float)Math.PI;
                case Direction.W:
                    return (3.0f / 2.0f * (float)Math.PI);
                default:
                    throw new Exception("Invalid direction");
            }
        }

        private MapVector GetForwardMoveVector()
        {
            switch (Facing)
            {
                case Direction.N:
                    return new MapVector(0, -1);
                case Direction.E:
                    return new MapVector(1, 0);
                case Direction.S:
                    return new MapVector(0, 1);
                case Direction.W:
                    return new MapVector(-1, 0);
                default:
                    throw new Exception("Invalid move vector");
            }
        }

        private MapVector GetBackwardMoveVector()
        {
            switch (Facing)
            {
                case Direction.N:
                    return new MapVector(0, 1);
                case Direction.E:
                    return new MapVector(-1, 0);
                case Direction.S:
                    return new MapVector(0, -1);
                case Direction.W:
                    return new MapVector(1, 0);
                default:
                    throw new Exception("Invalid move vector");
            }
        }

        private bool IsValidMove(MapVector newPosition)
        {
            return newPosition.InsideBoundary(MapGrid.GetLength(0), MapGrid.GetLength(1)) &&
                   MapGrid[newPosition.X, newPosition.Y] != Block.Solid;
        }

        private void UpdateMapGrid()
        {
            MapGrid[Position.X, Position.Y] = Block.Empty;
        }
    }
}
