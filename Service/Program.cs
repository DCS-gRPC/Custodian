﻿/* 
Custodian is a DCS server administration tool for Discord
Copyright (C) 2022 Jeffrey Jones

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RurouniJones.Custodian.Configuration.Util;
using Serilog;
using System.Runtime.InteropServices;

namespace RurouniJones.Custodian.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (OperatingSystem.IsWindows() && Environment.UserInteractive)
                    ConsoleProperties.DisableQuickEdit();
                Console.Title = $"Custodian Version {typeof(Worker).Assembly.GetName().Version}";

                // Setup the alternative configuration files instead of appsettings.json
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddYamlFile("configuration.yaml", false, true)
                    .AddYamlFile("configuration.development.yaml", true, true)
                    .Build();

                // Setup the logger configuration
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
                        services.AddHostedService<Worker>();
                        services.AddSingleton(x => {
                            var SocketConfig = new DiscordSocketConfig
                            {
                                LogLevel = LogSeverity.Debug,
                                GatewayIntents = GatewayIntents.AllUnprivileged
                            };
                            return new DiscordSocketClient(SocketConfig);
                        });
                        services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
                        services.AddSingleton<Core.Discord.InteractionHandler>();
                        services.AddSingleton<Core.Discord.Client>();
                        services.AddSingleton<Core.Dcs.Client>();
                        services.AddTransient<Core.Dcs.ChatService>();
                        services.AddTransient<Core.Dcs.EvalService>();
                        services.AddTransient<Core.Dcs.OutTextService>();
                        services.AddTransient<Core.Dcs.PlayerService>();
                        services.AddTransient<Core.Dcs.TransmitService>();
                        services.AddOpenTelemetryTracing((builder) => builder
                            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Custodian"))
                            .AddSource(nameof(Worker))
                            .AddSource(nameof(Core.Discord.InteractionHandler))
                            .AddSource(nameof(Core.Discord.Interactions.ChatInteraction))
                            .AddConsoleExporter()
                            .SetSampler(new AlwaysOnSampler())
                        );
                        services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(60));
                        services.AddOptions<Configuration.Application>()
                            .Bind(configuration.GetSection("Application"))
                            .ValidateDataAnnotationsRecursively();
                    })
                    .UseSerilog();

                if (OperatingSystem.IsWindows())
                    hostBuilder.UseWindowsService();

                var host = hostBuilder.Build();
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                if (!Environment.UserInteractive) return;
                Console.ReadKey();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    // Taken from
    // https://stackoverflow.com/questions/13656846/how-to-programmatic-disable-c-sharp-console-applications-quick-edit-mode
    internal static class ConsoleProperties
    {

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        private const int StdInputHandle = -10;

        private const uint QuickEdit = 0x0040;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        internal static bool DisableQuickEdit()
        {
            var consoleHandle = GetStdHandle(StdInputHandle);
            GetConsoleMode(consoleHandle, out var consoleMode);
            consoleMode &= ~QuickEdit;
            return SetConsoleMode(consoleHandle, consoleMode);
        }
    }
}
