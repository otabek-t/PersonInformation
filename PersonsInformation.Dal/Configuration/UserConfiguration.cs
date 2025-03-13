using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonsInformation.Dal.Entities;
using Telegram.Bot.Types;
using User = PersonsInformation.Dal.Entities.User;
using UserInfo = PersonsInformation.Dal.Entities.UserInfo;


namespace PersonsInformation.Dal.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("UsersInfo");

        builder.HasKey(u => u.BotUserId);

        builder.HasIndex(u => u.BotUserId).IsUnique();

        builder.HasIndex(u => u.ChatId).IsUnique();

        builder.HasOne(bu => bu.UserInfo)
            .WithOne(ui => ui.)
            .HasForeignKey<UserInfo>(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
