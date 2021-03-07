using Microsoft.EntityFrameworkCore;
using PMVOnline.Departments;
using PMVOnline.Files;
using PMVOnline.Tasks;
using PMVOnline.Users;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Users.EntityFrameworkCore;

namespace PMVOnline.EntityFrameworkCore
{
    public static class PMVOnlineDbContextModelCreatingExtensions
    {
        public static void ConfigurePMVOnline(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
             

            /* Configure your own tables/entities inside here */
            builder.Entity<Task>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "Tasks");  
                b.ConfigureExtraProperties();
            });
            builder.Entity<File>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "Files");
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
            });
            builder.Entity<TaskHistory>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskHistories");
                b.ConfigureByConvention(); 
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskComment>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskComments");
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskFollow>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskFollows");
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
            });

            builder.Entity<ReferenceTask>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "ReferenceTasks"); 
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskFile>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskFiles");
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskCommentFile>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskCommentFiles");
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
            });
            
            builder.Entity<TaskNotification>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskNotifications");
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
            });
        }
    }
}