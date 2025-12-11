using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Entities;

namespace Server.Contexts;

public partial class UserManagementAdditionalContext : DbContext
{
    public UserManagementAdditionalContext()
    {
    }

    public UserManagementAdditionalContext(DbContextOptions<UserManagementAdditionalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Applicationid).HasName("application_pkey");

            entity.ToTable("application");

            entity.Property(e => e.Applicationid).HasColumnName("applicationid");
            entity.Property(e => e.Applicationname).HasColumnName("applicationname");
            entity.Property(e => e.Requirestwofactor).HasColumnName("requirestwofactor");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
