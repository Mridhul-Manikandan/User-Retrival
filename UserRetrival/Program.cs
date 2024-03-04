using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using UserRetrival;

class Program
{
    static void Main()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Environment.SetEnvironmentVariable("AzureFunctionsJobHost__Logging__Console__IsEnabled", "false");

        try
        {
            CreateHostBuilder().Build().Run();  
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder() =>
        new HostBuilder()
            .ConfigureWebJobs(builder =>
            {
                builder.AddAzureStorageCoreServices();
                builder.AddTimers();
            })
            .ConfigureServices(ConfigureServices)
            .ConfigureLogging(ConfigureLogging);

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build());
    }

    private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.AddSerilog();
    }
}
