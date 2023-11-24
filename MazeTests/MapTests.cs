using Moq;
using System.Reflection;

namespace Maze.Test
{
    [TestClass]
    public class MapTests
    {
        [TestInitialize]
        public void CreateMapValidIMapProvider()
        {
            // Arrange
            var mockProvider = new Mock<IMapProvider>();
            Direction[,] directionMap = new Direction[,]
            {
                { Direction.E | Direction.S, Direction.W | Direction.S, Direction.E | Direction.S, Direction.W | Direction.S },
                { Direction.N | Direction.S, Direction.N | Direction.E, Direction.N | Direction.W, Direction.N | Direction.S },
                { Direction.N | Direction.E, Direction.E | Direction.W, Direction.W, Direction.N }
            };
            mockProvider.Setup(mp => mp.CreateMap()).Returns(directionMap);
            var map = new Map(mockProvider.Object);

            // Act
            map.CreateMap();

            // Assert
            // Should initialize all the fields properly and then house the variables since the map was defined correctly.
            Assert.AreEqual(9, map.Width);
            Assert.AreEqual(7, map.Height);
            Assert.AreEqual(Block.Empty, map.MapGrid[1, 1]);
            Assert.AreEqual(Block.Solid, map.MapGrid[1, 0]);
            Assert.AreEqual(Block.Solid, map.MapGrid[6, 6]);
            Assert.AreEqual(Block.Empty, map.MapGrid[5, 5]);
            Assert.IsNotNull(map.Goal);
            Assert.IsFalse(map.IsGameFinished);
            Assert.IsNotNull(map.Player);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMapNullIMapProvider()
        {
            // Arrange
            IMapProvider? provider = null;

            // Act
            Map map = new Map(provider);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CreateMapEvenHeightAndWidth()
        {
            // Arrange
            var mockProvider = new Mock<IMapProvider>();
            Direction[,] directionMap = new Direction[2, 3];

            // Fill the direction map to create the 2,3 maze
            // However it will end up as a 5x7 maze since we are adding *2 + 1 to achieve that map size. 
            directionMap[0, 0] = Direction.S;
            directionMap[0, 1] = Direction.S;
            directionMap[0, 2] = Direction.S;
            directionMap[1, 0] = Direction.N;
            directionMap[1, 1] = Direction.N;
            directionMap[1, 2] = Direction.N;

            mockProvider.Setup(mp => mp.CreateMap()).Returns(directionMap);
            var map = new Map(mockProvider.Object);

            // Act
            map.CreateMap();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CheckIfNoDeadEnd()
        {
            // Arrange
            // Should ensure that if there's no dead ends an exception is thrown
            var mockProvider = new Mock<IMapProvider>();
            Direction[,] directionMap = new Direction[,]
            {
                { Direction.S, Direction.S },
                { Direction.W, Direction.E }
            };
            mockProvider.Setup(mp => mp.CreateMap()).Returns(directionMap);
            var map = new Map(mockProvider.Object);

            // Act
            map.CreateMap();
        }

        [TestMethod]
        public void IsDeadEndValidPosition()
        {
            // checks to see if the dead ends are valid
            // Arrange
            var mockProvider = new Mock<IMapProvider>();
            Direction[,] directionMap = new Direction[,]
            {
               { Direction.E | Direction.S, Direction.W | Direction.S },
               { Direction.N, Direction.N }
            };
            mockProvider.Setup(mp => mp.CreateMap()).Returns(directionMap);
            var map = new Map(mockProvider.Object);
            map.CreateMap();
        }

        [TestMethod]
        public void CalculateDistanceValidPositions()
        {
            // Arrange
            var mockProvider = new Mock<IMapProvider>();
            // 9x7 map
            Direction[,] directionMap = new Direction[,]
            {
                { Direction.E | Direction.S, Direction.W | Direction.S, Direction.E | Direction.S, Direction.W | Direction.S },
                { Direction.N | Direction.S, Direction.N | Direction.E, Direction.N | Direction.W, Direction.N | Direction.S },
                { Direction.N | Direction.E, Direction.E | Direction.W, Direction.W, Direction.N }
            };
            mockProvider.Setup(mp => mp.CreateMap()).Returns(directionMap);
            var map = new Map(mockProvider.Object);
            map.CreateMap();

            MapVector position1 = new MapVector(1, 1);
            MapVector position2 = new MapVector(4, 5); // coordinates

            // Act using the non public fields 
            // uses the distance formulae to calculate the distance between the two points so it should be the furthest point relative to those two points here
            int distance = (int)typeof(Map).GetMethod("CalculateDistance", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(map, new object[] { position1, position2 });

            // Assert
            Assert.AreEqual(7, distance);
        }
    }
}