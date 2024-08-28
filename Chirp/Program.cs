﻿using System;
using System.IO;

//Use dotnet run -- read to run the code
namespace Chirp {
    class App {
        static void Main(string[] args){
            if (args.Length > 0 && args[0] == "read")
            {
                string[] lines = File.ReadAllLines("chirp_cli_db.csv");

                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    string author = parts[0];
                    string message = string.Join(",", parts[1]);
                    string timestamp = ConnectTimestamp(parts[2]);
                    Console.WriteLine($"{author} @ {timestamp}: {message}");
                }
            }
        }

        static string ConnectTimestamp(string timestamp)
        {
            return null;
        }
    }
}