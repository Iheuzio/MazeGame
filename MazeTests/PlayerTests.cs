namespace Maze.Tests
{
    [TestClass]
    public class PlayerTests
    {
        private Block[,]? mapGrid;
        private Player? player;

        [TestInitialize]
        public void Setup()
        {
            MapVector startPosition = new MapVector(1, 2);
            Direction initialFacing = Direction.N;
            // south will dictate movement for forward and backward here
            mapGrid = new Block[3, 5] {
                { Block.Solid, Block.Solid, Block.Solid, Block.Solid, Block.Solid },
                { Block.Solid, Block.Empty, Block.Empty, Block.Empty, Block.Solid },
                { Block.Solid, Block.Solid, Block.Solid, Block.Solid, Block.Solid}
            };
            player = new Player(startPosition, initialFacing, mapGrid);
        }

        [TestMethod]
        public void ConstructorNullStartPosition()
        {
            // Arrange
            MapVector? startPosition = null;
            Direction initialFacing = Direction.N;

            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new Player(startPosition, initialFacing, mapGrid));
        }

        [TestMethod]
        public void ConstructorStartPositionOutsideMapGrid()
        {
            // Arrange
            MapVector startPosition = new MapVector(4, 4);
            Direction initialFacing = Direction.N;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => new Player(startPosition, initialFacing, mapGrid));
        }

        [TestMethod]
        public void ConstructorStartPositionSolidBlock()
        {
            // Arrange
            MapVector startPosition = new MapVector(0, 0);
            Direction initialFacing = Direction.N;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => new Player(startPosition, initialFacing, mapGrid));
        }

        [TestMethod]
        public void ConstructorInvalidInitialFacing()
        {
            // Arrange
            MapVector startPosition = new MapVector(1, 1);
            Direction initialFacing = Direction.None;

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() => new Player(startPosition, initialFacing, mapGrid));
        }

        [TestMethod]
        public void MoveForwardValidMoveUpdatesPosition()
        {
            // Arrange
            MapVector expectedPosition = new MapVector(1, 3);

            // Act
            player.TurnRight();
            player.TurnRight();
            player.MoveForward();

            // Assert
            Assert.AreEqual(expectedPosition, player.Position);
        }

        [TestMethod]
        public void MoveBackwardValidMoveUpdatesPosition()
        {
            // Arrange
            MapVector expectedPosition = new MapVector(1, 1);

            // Act
            player.TurnRight();
            player.TurnRight();
            player.MoveBackward();

            // Assert
            Assert.AreEqual(expectedPosition, player.Position);
        }

        [TestMethod]
        public void TurnLeftValidMoveUpdatesFacing()
        {
            // Arrange
            Direction expectedFacing = Direction.W;

            // Act
            player.TurnLeft();

            // Assert
            Assert.AreEqual(expectedFacing, player.Facing);
        }

        [TestMethod]
        public void MoveIntoSolidBlock()
        {
            // Arrange
            MapVector expectedPosition = new MapVector(1, 2);

            // Act
            player.TurnRight();
            player.MoveForward();

            // Assert
            Assert.AreEqual(expectedPosition, player.Position);
        }

        [TestMethod]
        public void TurnRightValidMoveUpdatesFacing()
        {
            // Arrange
            Direction expectedFacing = Direction.E;

            // Act
            player.TurnRight();

            // Assert
            Assert.AreEqual(expectedFacing, player.Facing);
        }

        [TestMethod]
        public void TestTurnLeftAndTurnRight()
        {
            // Arrange
            MapVector startPosition = new MapVector(1, 2);
            Direction initialFacing = Direction.N;
            Block[,] mapGrid1 = new Block[3, 5] {
                { Block.Solid, Block.Solid, Block.Solid, Block.Solid, Block.Solid },
                { Block.Solid, Block.Empty, Block.Empty, Block.Empty, Block.Solid },
                { Block.Solid, Block.Solid, Block.Solid, Block.Solid, Block.Solid}
            };
            Player player1 = new Player(startPosition, initialFacing, mapGrid1);

            // Act & Assert
            player1.TurnLeft();
            Assert.AreEqual(player1.Facing, Direction.W);

            player1.TurnRight();
            Assert.AreEqual(player1.Facing, Direction.N);

            player1.TurnLeft();
            Assert.AreEqual(player1.Facing, Direction.W);

            player1.TurnRight();
            Assert.AreEqual(player1.Facing, Direction.N);

            player1.TurnRight();
            Assert.AreEqual(player1.Facing, Direction.E);

            player1.TurnRight();
            Assert.AreEqual(player1.Facing, Direction.S);

            player1.TurnLeft();
            Assert.AreEqual(player1.Facing, Direction.E);

            player1.TurnLeft();
            Assert.AreEqual(player1.Facing, Direction.N);
        }
    }
}