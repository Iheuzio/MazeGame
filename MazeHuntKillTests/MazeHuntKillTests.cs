namespace MazeHuntKillTests;
using Maze;
using MazeHuntKill;

[TestClass]
public class MazeRecursionTests
{
    [TestMethod]
    public void TestCreateMap()
    {
        // provider to test
        HuntKillMazeGen mazeGen = new HuntKillMazeGen();
        int width = 21;
        int height = 19;
        Map map = new Map(mazeGen);
        map.CreateMap(width, height);

        // create a multi-dimensional array to store the data
        int[,] mazeArray = new int[height / 2, width / 2];
        for (int i = 0; i < (height / 2); i++)
        {
            for (int j = 0; j < (width / 2); j++)
            {
                mazeArray[i, j] = (int)map.MapGrid[i, j];
            }
        }

        // check if the size is well represented based on the amount of values present in each row and column
        bool isSizeWellRepresented = true;
        for (int i = 0; i < (height / 2); i++)
        {
            if (mazeArray.GetLength(1) != (width / 2))
            {
                isSizeWellRepresented = false;
                break;
            }
        }
        for (int j = 0; j < (width / 2); j++)
        {
            if (mazeArray.GetLength(0) != (height / 2))
            {
                isSizeWellRepresented = false;
                break;
            }
        }

        Assert.IsTrue(isSizeWellRepresented);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestCreateMapWithEvenWidth()
    {
        HuntKillMazeGen mazeGen = new HuntKillMazeGen();
        Direction[,] maze = mazeGen.CreateMap(4, 5);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestCreateMapWithEvenHeight()
    {
        HuntKillMazeGen mazeGen = new HuntKillMazeGen();
        Direction[,] maze = mazeGen.CreateMap(5, 4);
    }

    [TestMethod]
    public void TestCreateMapDefault()
    {
        HuntKillMazeGen mazeGen = new HuntKillMazeGen();
        Direction[,] maze = mazeGen.CreateMap();

        Assert.AreEqual(2, maze.GetLength(0));
        Assert.AreEqual(2, maze.GetLength(1));
    }

    [TestMethod]
    public void TestMapsAreRandom()
    {
        HuntKillMazeGen mazeGen = new HuntKillMazeGen(1);
        Direction[,] maze1 = mazeGen.CreateMap(7, 5);

        HuntKillMazeGen mazeGen1 = new HuntKillMazeGen(2);
        Direction[,] maze2 = mazeGen1.CreateMap(7, 5);

        bool areDifferent = false;

        for (int i = 0; i < maze1.GetLength(0); i++)
        {
            for (int j = 0; j < maze1.GetLength(1); j++)
            {
                if (maze1[i, j] != maze2[i, j])
                {
                    areDifferent = true;
                    break;
                }
            }
        }
        Assert.IsTrue(areDifferent);
    }

    [TestMethod]
    public void TestSeedGen()
    {
        HuntKillMazeGen mazeGen = new HuntKillMazeGen(1);
        Direction[,] maze1 = mazeGen.CreateMap(7, 5);

        HuntKillMazeGen mazeGen2 = new HuntKillMazeGen(1);
        Direction[,] maze2 = mazeGen2.CreateMap(7, 5);
        bool areDifferent = false;

        for (int i = 0; i < maze1.GetLength(0); i++)
        {
            for (int j = 0; j < maze1.GetLength(1); j++)
            {
                if (maze1[i, j] != maze2[i, j])
                {
                    areDifferent = true;
                    break;
                }
            }
        }
        Assert.IsFalse(areDifferent);
    }

    [TestMethod]
    public void TestExactCreateMapSeed()
    {
        HuntKillMazeGen mazeGen = new HuntKillMazeGen(1);
        Direction[,] maze = mazeGen.CreateMap();

        Assert.AreEqual(Direction.S, maze[0, 0]);
        Assert.AreEqual(Direction.S, maze[0, 1]);
        Assert.AreEqual(Direction.N | Direction.E, maze[1, 0]);
        Assert.AreEqual(Direction.N | Direction.W, maze[1, 1]);
    }

    // test map vector creation to ensure that there are no empty directions
    [TestMethod]
    public void TestMapVectorCreation()
    {
        HuntKillMazeGen mazeGen = new HuntKillMazeGen();
        Direction[,] maze = mazeGen.CreateMap(5, 5);

        bool isMapVectorCreated = true;
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                if (maze[i, j] == Direction.None)
                {
                    isMapVectorCreated = false;
                    break;
                }
            }
        }
        Assert.IsTrue(isMapVectorCreated);
    }
}
