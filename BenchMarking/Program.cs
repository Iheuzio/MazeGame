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
            // Specify the file paths for logging results
            string logFilePath = "benchmark_log.txt";
            string csvFilePath = "benchmark_results.csv";

            // Run benchmarks for different maze sizes (odd numbers starting from 5x5 up to 1000x1000)
            for (int size = 5; size <= 255; size += 2)
            {
                BenchmarkForMazeSize(size, logFilePath, csvFilePath);
            }

            Console.WriteLine("Benchmarking complete. Check the log file and CSV file for details.");
        }

        static void BenchmarkForMazeSize(int size, string logFilePath, string csvFilePath)
        {
            // Create an instance of each IMapProvider algorithm
            IMapProvider recursiveMazeProvider = new RecursiveMazeGen();
            IMapProvider huntKillMazeProvider = new HuntKillMazeGen();

            Console.WriteLine($"Running benchmarks for maze size {size}...");

            // Run benchmarks for RecursiveMazeGen
            BenchmarkAlgorithm(recursiveMazeProvider, $"RecursiveMazeGen-{size}", size, size, logFilePath, csvFilePath);

            // Run benchmarks for HuntKillMazeGen
            BenchmarkAlgorithm(huntKillMazeProvider, $"HuntKillMazeGen-{size}", size, size, logFilePath, csvFilePath);
        }

        static void BenchmarkAlgorithm(IMapProvider mapProvider, string algorithmName, int width, int height, string logFilePath, string csvFilePath)
        {
            Console.WriteLine($"Benchmarking {algorithmName}...");

            // Benchmark in Debug mode
            Console.WriteLine("Debug Mode:");
            TimeAndLog(mapProvider, () => mapProvider.CreateMap(width, height), $"Debug-{algorithmName}", logFilePath, csvFilePath, width, height);

            // Benchmark in Release mode
            Console.WriteLine("Release Mode:");
            TimeAndLog(mapProvider, () => mapProvider.CreateMap(width, height), $"Release-{algorithmName}", logFilePath, csvFilePath, width, height);
        }

        static void TimeAndLog(IMapProvider mapProvider, Action action, string testName, string logFilePath, string csvFilePath, int width, int height)
        {
            // Time the action using the TimeIt method
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
