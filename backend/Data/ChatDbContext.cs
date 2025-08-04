using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }
        public DbSet<Bot> Bots => Set<Bot>();
        public DbSet<Message> Messages => Set<Message>();

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        var config = new ConfigurationBuilder()
        //            .SetBasePath(Directory.GetCurrentDirectory())
        //            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //            .Build();
        //        optionsBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        //    }
        //}


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Bot>()
        //        .HasMany(b => b.Messages)
        //        .WithOne(m => m.Bot)
        //        .HasForeignKey(m => m.BotId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //    modelBuilder.Entity<Message>()
        //        .HasOne(m => m.Bot)
        //        .WithMany(b => b.Messages)
        //        .HasForeignKey(m => m.BotId);
        //}

    }
}
