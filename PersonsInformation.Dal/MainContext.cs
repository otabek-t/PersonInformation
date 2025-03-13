using Microsoft.EntityFrameworkCore;
using PersonsInformation.Dal.Configuration;
using PersonsInformation.Dal.Entities;

namespace MyFirstEBot;

public class MainContext : DbContext
{
    public DbSet<UserInfo> UserInfos{ get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server = DESKTOP-UITR0QQ\\SQLEXPRESS;Database=UserDb;User Id=sa;Password=1");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserInfoConfiguration());

    }


}
