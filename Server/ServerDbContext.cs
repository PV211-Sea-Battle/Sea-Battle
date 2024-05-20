using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;


namespace Server
{
    public class ServerDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<Cell> Cell { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<CompletedGame> CompletedGame { get; set; }
        public ServerDbContext() : base(GenerateOptions())
        {
            Database.EnsureCreated();
        }
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public static DbContextOptions<ServerDbContext> GenerateOptions()
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
            var options = new DbContextOptionsBuilder<ServerDbContext>()
                .UseSqlServer(config.GetConnectionString("SqlClient")).Options;
            return options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Field>().HasMany(f => f.Cells);

            builder.Entity<Game>(item =>
            {
                item.HasOne(item => item.HostUser)
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction);
                item.HasOne(item => item.ClientUser)
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction);
                item.HasOne(item => item.HostField)
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction);
                item.HasOne(item => item.ClientField)
                    .WithMany()
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<User>().HasIndex(u => u.Login).IsUnique();
        }
    }
}
