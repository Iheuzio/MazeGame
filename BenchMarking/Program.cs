using MazeHuntKill;
using MazeRecursion;
using System;
using System.Diagnostics;
using System.IO;

namespace Maze
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set the maze size for testing
            int mazeWidth = 15;
            int mazeHeight = 15;

            // Specify the file path for logging results
            string logFilePath = "benchmark_log.txt";

            // Create an instance of each IMapProvider algorithm
            IMapProvider recursiveMazeProvider = new RecursiveMazeGen();
            IMapProvider huntKillMazeProvider = new HuntKillMazeGen();

            // Run benchmarks for RecursiveMazeGen
            BenchmarkAlgorithm(recursiveMazeProvider, "RecursiveMazeGen", mazeWidth, mazeHeight, logFilePath);

            // Run benchmarks for HuntKillMazeGen
            BenchmarkAlgorithm(huntKillMazeProvider, "HuntKillMazeGen", mazeWidth, mazeHeight, logFilePath);

            Console.WriteLine("Benchmarking complete. Check the log file for details.");
        }

        static void BenchmarkAlgorithm(IMapProvider mapProvider, string algorithmName, int width, int height, string logFilePath)
        {
            Console.WriteLine($"Benchmarking {algorithmName}...");

            // Benchmark in Debug mode
            Console.WriteLine("Debug Mode:");
            TimeAndLog(() => mapProvider.CreateMap(width, height), $"Debug-{algorithmName}", logFilePath);

            // Benchmark in Release mode
            Console.WriteLine("Release Mode:");
            TimeAndLog(() => mapProvider.CreateMap(width, height), $"Release-{algorithmName}", logFilePath);
        }

        static void TimeAndLog(Action action, string testName, string logFilePath)
        {
            // Time the action using the TimeIt method
            TimeSpan elapsedTime = TimeIt(action);

            // Log the result to the console
            Console.WriteLine($"{testName} Time: {elapsedTime.TotalMilliseconds} ms");

            // Write the result to the log file
            WriteToFile(logFilePath, $"{testName} Time: {elapsedTime.TotalMilliseconds} ms");
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
    }
}
