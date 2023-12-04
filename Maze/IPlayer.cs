namespace Maze
{
    /// <summary>
    /// Represents a Player in the maze
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The direction the player is currently facing
        /// </summary>
        Direction Facing { get; }
        /// <summary>
        /// The current position of the player
        /// </summary>
        MapVector Position { get; }
        /// <summary>
        /// Start position of the player
        /// </summary>
        int StartX { get; }
        /// <summary>
        /// Start position of the player
        /// </summary>
        int StartY { get; }

        float GetRotation();
        /// <summary>
        /// Moves the player backwards along its current direction. If the move is not legal, the player does not move
        /// </summary>
        void MoveBackward();
        /// <summary>
        /// Moves the player forwards along its current direction. If the move is not legal, the player does not move
        /// </summary>
        void MoveForward();
        /// <summary>
        /// Turns the player left based on its direction
        /// </summary>
        void TurnLeft();
        /// <summary>
        /// Turns the player right based on its direction
        /// </summary>
        void TurnRight();
    }
}