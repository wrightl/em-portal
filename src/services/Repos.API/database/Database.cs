using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;

namespace EmPortal.Repos_API;

public static class EntityFrameworkExtensions
{
    public static async Task ApplyMigrations(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(EntityFrameworkExtensions));

        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        logger.LogInformation("Start migrating database. Version: {Version}", version);

        var retryCount = 0;
        while (retryCount <= 30)
        {
            if (retryCount >= 1)
                logger.LogInformation("Waiting for databases to be ready. Retry count: {RetryCount}", retryCount);

            try
            {
                var dbContext = scope.ServiceProvider.GetService<TicketContext>() ??
                                throw new UnreachableException("Missing DbContext.");

                var strategy = dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(() =>
                {
                    // dbContext.Database.MigrateAsync(cancellationToken);
                    return dbContext.Database.EnsureCreatedAsync();
                });

                logger.LogInformation("Seeding database");

                if (dbContext.Tickets.Count() == 0)
                {
                    dbContext.Tickets.Add(new SupportTicket
                    {
                        Title = "New Test Ticket",
                        Description = "Testing Ticket Db"
                    });

                    await dbContext.SaveChangesAsync(cancellationToken);
                }
                logger.LogInformation("Created dummy data in database.");
                break;
            }
            catch (SqlException ex) when (ex.Message.Contains("an error occurred during the pre-login handshake"))
            {
                // Known error in Aspire, when SQL Server is not ready. See: https://github.com/dotnet/aspire/issues/1023
                retryCount++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            catch (SocketException ex) when (ex.Message.Contains("Invalid argument"))
            {
                // Known error in Aspire, when SQL Server is not ready See: https://github.com/dotnet/aspire/issues/1023
                retryCount++;
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying migrations.");

                // Wait for the logger to flush
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}

public class TicketContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SupportTicket> Tickets => Set<SupportTicket>();
}

public sealed class SupportTicket
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
}