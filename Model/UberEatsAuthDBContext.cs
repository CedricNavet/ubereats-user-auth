using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ubereats_user_auth.Model
{
    public partial class UberEatsAuthDBContext : DbContext
    {
        public UberEatsAuthDBContext()
        {
        }

        public UberEatsAuthDBContext(DbContextOptions<UberEatsAuthDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "French_CI_AS");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("First_Name");

                entity.Property(e => e.IsValid).HasColumnName("Is_valid");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("Last_Name");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.HasOne(d => d.MentoringNavigation)
                    .WithMany(p => p.InverseMentoringNavigation)
                    .HasForeignKey(d => d.Mentoring)
                    .HasConstraintName("FK_User_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
