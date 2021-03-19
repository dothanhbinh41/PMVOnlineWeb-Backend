using Microsoft.EntityFrameworkCore;
using PMVOnline.Departments;
using PMVOnline.Files;
using PMVOnline.Guides;
using PMVOnline.Tasks;
using PMVOnline.Users;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Users.EntityFrameworkCore;

namespace PMVOnline.EntityFrameworkCore
{
    /* This is your actual DbContext used on runtime.
     * It includes only your entities.
     * It does not include entities of the used modules, because each module has already
     * its own DbContext class. If you want to share some database tables with the used modules,
     * just create a structure like done for AppUser.
     *
     * Don't use this DbContext for database migrations since it does not contain tables of the
     * used modules (as explained above). See PMVOnlineMigrationsDbContext for migrations.
     */
    [ConnectionStringName("Default")]
    public class PMVOnlineDbContext : AbpDbContext<PMVOnlineDbContext>
    {
        public DbSet<Guide> Guides { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskAction> TaskActions { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskFollow> TaskFollows { get; set; }
        public DbSet<ReferenceTask> ReferenceTasks { get; set; }
        public DbSet<TaskFile> TaskFiles { get; set; }
        public DbSet<TaskCommentFile> TaskCommentFiles { get; set; }

        /* Add DbSet properties for your Aggregate Roots / Entities here.
         * Also map them inside PMVOnlineDbContextModelCreatingExtensions.ConfigurePMVOnline
         */

        public PMVOnlineDbContext(DbContextOptions<PMVOnlineDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            //builder.Entity<AppUser>(b =>
            //{
            //    b.ToTable(PMVOnlineConsts.DbTablePrefix + "AppUsers");
            //    b.ConfigureByConvention();
            //    b.ConfigureAuditedAggregateRoot();
            //    b.ConfigureExtraProperties();
            //});
            /* Configure the shared tables (with included modules) here */

            /* Configure your own tables/entities inside the ConfigurePMVOnline method */

            builder.ConfigurePMVOnline();
        }
    }
}
