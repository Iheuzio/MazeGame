namespace Maze.Tests
{
    [TestClass]
    public class MapVectorTests
    {
        [TestMethod]
        public void MapVectorIsValid_ValidVector_ReturnsTrue()
        {
            // Arrange
            MapVector v1 = new MapVector(1, 2);

            // Act
            bool result = v1.IsValid;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MapVectorIsValid_InvalidVector_ReturnsFalse()
        {
            // Arrange
            MapVector v1 = new MapVector(-1, 2);

            // Act
            bool result = v1.IsValid;

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MapVectorInsideBoundary_VectorWithinBoundary_ReturnsTrue()
        {
            // Arrange
            MapVector v1 = new MapVector(1, 2);

            // Act
            bool result = v1.InsideBoundary(4, 5);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MapVectorInsideBoundary_VectorOutsideBoundary_ReturnsFalse()
        {
            // Arrange
            MapVector v1 = new MapVector(5, 6);

            // Act
            bool result = v1.InsideBoundary(4, 5);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MapVectorMagnitude_ReturnsCorrectValue()
        {
            // Arrange
            MapVector v1 = new MapVector(3, 4);

            // Act
            double result = v1.Magnitude();

            // Assert
            Assert.AreEqual(result, 5);
        }

        [TestMethod]
        public void MapVectorOperatorAdd_ReturnsSumVector()
        {
            // Arrange
            MapVector v1 = new MapVector(1, 2);
            MapVector v2 = new MapVector(3, 4);

            // Act
            MapVector result = v1 + v2;

            // Assert
            Assert.AreEqual(result.X, 4);
            Assert.AreEqual(result.Y, 6);
        }

        [TestMethod]
        public void MapVectorOperatorSubtract_ReturnsDifferenceVector()
        {
            // Arrange
            MapVector v1 = new MapVector(1, 2);
            MapVector v2 = new MapVector(3, 4);

            // Act
            MapVector result = v2 - v1;

            // Assert
            Assert.AreEqual(result.X, 2);
            Assert.AreEqual(result.Y, 2);
        }

        [TestMethod]
        public void MapVectorOperatorMultiply_ReturnsScaledVector()
        {
            // Arrange
            MapVector v1 = new MapVector(1, 2);
            int scalar = 3;

            // Act
            MapVector result = v1 * scalar;

            // Assert
            Assert.AreEqual(result.X, 3);
            Assert.AreEqual(result.Y, 6);
        }

        [TestMethod]
        public void MapVectorCast_ReturnsCorrectVector()
        {
            // Arrange
            Direction direction = Direction.N;

            // Act
            MapVector result = direction;

            // Assert
            Assert.AreEqual(result.X, 0);
            Assert.AreEqual(result.Y, -1);     //N is -1 in Y direction
        }
    }
}