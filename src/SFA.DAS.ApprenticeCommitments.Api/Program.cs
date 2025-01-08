using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using System;

namespace SFA.DAS.ApprenticeCommitments.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNServiceBusContainer();
    }
}