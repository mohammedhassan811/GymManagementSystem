using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name)
                   .HasColumnType("Varchar")
                   .HasMaxLength(30);
            builder.Property(x => x.Email)
                  .HasColumnType("Varchar")
                  .HasMaxLength(100);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();

            builder.ToTable("Members", tb =>
            {
                tb.HasCheckConstraint(
                    "EmailCheck",
                    "Email LIKE '_%@_%._%'");

                tb.HasCheckConstraint(
                    "PhoneCheck",
                    "Phone LIKE '010%' OR " +
                    "Phone LIKE '011%' OR " +
                    "Phone LIKE '012%' OR " +
                    "Phone LIKE '015%'");
            });

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(x => x.Street).HasColumnName("Street").HasColumnType("Varchar").HasMaxLength(30);
                address.Property(x => x.City).HasColumnName("City").HasColumnType("Varchar").HasMaxLength(30);
            });

        }
    }
}
