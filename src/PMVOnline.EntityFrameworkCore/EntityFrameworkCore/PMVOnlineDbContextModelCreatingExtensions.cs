﻿using Microsoft.EntityFrameworkCore;
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
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });
            builder.Entity<File>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "Files");
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });
            builder.Entity<TaskAction>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskActions");
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskComment>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskComments");
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskFollow>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskFollows");
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });

            builder.Entity<ReferenceTask>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "ReferenceTasks");
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskFile>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskFiles");
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });

            builder.Entity<TaskCommentFile>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "TaskCommentFiles");
                b.ConfigureByConvention();
                b.ConfigureAuditedAggregateRoot();
                b.ConfigureExtraProperties();
            });

            builder.Entity<AppUser>(b =>
            {
                b.ToTable(PMVOnlineConsts.DbTablePrefix + "AppUsers"); //Sharing the same table "AbpUsers" with the IdentityUser  
                b.ConfigureByConvention();
                b.ConfigureExtraProperties();
                b.ConfigureAbpUser();
                b.ConfigureAuditedAggregateRoot();
                /* Configure mappings for your additional properties
                 * Also see the PMVOnlineEfCoreEntityExtensionMappings class
                 */
            });


        }
    }
}