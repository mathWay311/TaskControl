using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TaskControl.DAL
{ 
    public class TaskDBContext : DbContext
    {
        public DbSet<Entity.Task> Task { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlite(connectionString);
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }

        }



    }
}
