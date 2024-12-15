using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Chirp.Tests
{
    public class TestFixture<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Uri BaseAddress { get; }
        public HttpClient Client { get; }
        public Process? serverProcess;

        public TestFixture()
        {
            BaseAddress = new Uri("http://localhost:5273");
            Client = CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = BaseAddress
            });
        }

        public async Task EnsureServerIsReady()
        {
        
            serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --project ../../../../../src/Chirp.Web",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var tcs = new TaskCompletionSource<bool>();

  serverProcess.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    Console.WriteLine(args.Data);
                    //checks if the server is ready
                    if (args.Data.Contains("Application started and listening on port 5273"))
                    {
                        tcs.SetResult(true);
                    }
                }
            };
        

            serverProcess.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

            serverProcess.Start();
            serverProcess.BeginOutputReadLine();
            serverProcess.BeginErrorReadLine();

            await tcs.Task;
        }

        public void StopServer()
        {
            if (serverProcess != null && !serverProcess.HasExited)
            {
                serverProcess.Kill(true); //force kill the process
                serverProcess.WaitForExit(5000); //give it 5 seconds to gracefully exit
                serverProcess.Dispose();
                
            }
            

           
        }
    }
}