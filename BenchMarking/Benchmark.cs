using MazeHuntKill;
using MazeRecursion;
using System.Diagnostics;

namespace Maze
{
    class Benchmark
    {
        static void Main(string[] args)
        {
            // Specify the file paths for logging results
            string logFilePath = "benchmark_log.txt";
            string csvFilePath = "benchmark_results.csv";

            // Run benchmarks for different maze sizes (odd numbers starting from 5x5 up to 1000x1000)
            for (int size = 5; size <= 255; size += 2)
            {
                BenchmarkForMazeSize(size, logFilePath, csvFilePath);
            }

            // just prints the current path as a hyperlink so its easier to navigate to
            Console.WriteLine($"Log file: \u001b]8;;file://{Path.GetFullPath(logFilePath)}\u0007{Path.GetFullPath(logFilePath)}\u001b]8;;\u0007");
            Console.WriteLine($"CSV file: \u001b]8;;file://{Path.GetFullPath(csvFilePath)}\u0007{Path.GetFullPath(csvFilePath)}\u001b]8;;\u0007");

            Console.WriteLine("Benchmarking complete. Check the log file and CSV file for details.");
        }

        static void BenchmarkForMazeSize(int size, string logFilePath, string csvFilePath)
        {
            // Create an instance of each IMapProvider algorithm
            IMapProvider recursiveMazeProvider = new RecursiveMazeGen();
            IMapProvider huntKillMazeProvider = new ImprovedMazeHuntKillGen();
            
            // initial hunt and kill:
            // IMapProvider huntKillMazeProvider = new HuntKillMazeGen();

            Console.WriteLine($"Running benchmarks for maze size {size}...");
#if DEBUG
            string deployment = "Debug";
#else
            string deployment = "Release";
#endif

            // Run benchmarks for RecursiveMazeGen
            BenchmarkAlgorithm(recursiveMazeProvider, $"RecursiveMazeGen-{size}", size, size, logFilePath, csvFilePath, deployment);

            // Run benchmarks for HuntKillMazeGen
            BenchmarkAlgorithm(huntKillMazeProvider, $"HuntKillMazeGen-{size}", size, size, logFilePath, csvFilePath, deployment);
        }

        static void BenchmarkAlgorithm(IMapProvider mapProvider, string algorithmName, int width, int height, string logFilePath, string csvFilePath, string deployment)
        {
            Console.WriteLine($"Benchmarking {algorithmName}...");
            Console.WriteLine("");

            // Run benchmarks for CreateMap
            TimeAndLog(() => mapProvider.CreateMap(width, height), $"{deployment}-{algorithmName}", logFilePath, csvFilePath, width, height);
        }

        static void TimeAndLog(Action action, string testName, string logFilePath, string csvFilePath, int width, int height)
        {
            TimeSpan elapsedTime = TimeIt(action);

            // Log the result to the console
            Console.WriteLine($"{testName} Time: {elapsedTime.TotalMilliseconds} ms, Maze Size: {width}x{height}");

            // Write the result to the log file
            WriteToFile(logFilePath, $"{testName} Time: {elapsedTime.TotalMilliseconds} ms, Maze Size: {width}x{height}");

            // Write the result to the CSV file
            WriteToCsv(csvFilePath, $"{testName},{elapsedTime.TotalMilliseconds},{width},{height}");
        }

        static TimeSpan TimeIt(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action.Invoke();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        static void WriteToFile(string path, string data)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(path))
                {
                    writer.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to file: {ex.Message}");
            }
        }

        static void WriteToCsv(string path, string data)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(path))
                {
                    writer.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to CSV file: {ex.Message}");
            }
        }
    }
}
