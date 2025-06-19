using Application.Abstractions.Data;
using Domain.Plans;
using Domain.Todos;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<TodoItem> TodoItems { get; set; }

    public DbSet<Plan> Plans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}

// 添加设计时工厂
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        string? assemblyLocation = Path.GetDirectoryName(typeof(ApplicationDbContext).Assembly.Location) ?? throw new InvalidOperationException("Unable to determine the assembly location.");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(assemblyLocation)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // 使用与您的项目相同的数据库提供程序
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Database"));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
