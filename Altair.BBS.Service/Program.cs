using System;
using Altair.BBS.Service.Services;
using Serilog;
using Serilog.Core;

namespace Altair.BBS.Service // Note: actual namespace depends on the project name.
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var loggers = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .Enrich.FromLogContext();
            var telnet = new TelnetServer(loggers.CreateLogger());
            telnet.Start();
            
            Console.ReadLine();
        }
    }
}