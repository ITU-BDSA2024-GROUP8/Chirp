﻿using System;


//Use dotnet run -- read to run the code
namespace Chirp.CLI.Client {
    class App {
        static void Main(string[] args){
            if (args.Length > 0 && args[0] == "read")
            {
                string[] lines = File.ReadAllLines("../SimpleDB/Data/Data.csv");
                
                for (int i = 1; i < lines.Length; i++)
                {
                    var parts = lines[i].Split(',');
                    string author = parts[0];
                    string message = parts[1];
                    string timestamp = FromUnixTimeMilliseconds(long.Parse(parts[2])).ToString("yyyy-MM-dd HH:mm:ss");
                    Console.WriteLine($"{author} @ {timestamp}: {message}");
                }
            }
        }

        private static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }
    }
}