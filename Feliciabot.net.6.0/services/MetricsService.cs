using Microsoft.Extensions.Hosting;
using Prometheus;

namespace Feliciabot.net._6._0.services
{
    public class MetricsService : BackgroundService
    {
        private static readonly Counter SlashCommandsExecuted = Metrics.CreateCounter(
            "feliciabot_slash_commands_executed_total",
            "Total number of slash commands executed",
            new[] { "command_name" }
        );

        private static readonly Counter CommandsExecuted = Metrics.CreateCounter(
            "feliciabot_commands_executed_total",
            "Total number of commands executed",
            new[] { "command_name" }
        );

        private static readonly Counter CommandErrors = Metrics.CreateCounter(
            "feliciabot_command_errors_total",
            "Total number of command execution errors",
            new[] { "command_name" }
        );

        private static readonly Gauge GuildCount = Metrics.CreateGauge(
            "feliciabot_guild_count",
            "Total number of guilds the bot is in"
        );

        private static readonly Gauge UserCount = Metrics.CreateGauge(
            "feliciabot_user_count",
            "Total number of users the bot is serving"
        );

        private KestrelMetricServer? _metricServer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _metricServer = new KestrelMetricServer(port: 9300); // Expose at :9300/metrics
            _metricServer.Start();
            return Task.CompletedTask;
        }

        public void IncSlashCommand(string commandName)
        {
            SlashCommandsExecuted.WithLabels(commandName).Inc();
        }

        public void IncCommand(string commandName)
        {
            CommandsExecuted.WithLabels(commandName).Inc();
        }

        public void IncCommandError(string commandName)
        {
            CommandErrors.WithLabels(commandName).Inc();
        }

        public void SetGuildCount(int count)
        {
            GuildCount.Set(count);
        }

        public void SetUserCount(int count)
        {
            UserCount.Set(count);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _metricServer?.Stop();
            return base.StopAsync(cancellationToken);
        }
    }
}
