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
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(x => x.Id);

            builder.Property(x => x.CreatedAt)
                   .HasColumnName("BookingDate")
                   .HasDefaultValueSql("GETDATE()");

            builder.HasKey(x => new { x.SessionsId, x.MemberId });

            builder.HasOne(b => b.Session)
                   .WithMany(s => s.SessionMembers)
                   .HasForeignKey(b => b.SessionsId);

            builder.HasOne(b => b.Member)
                   .WithMany(m => m.MemberSessions)
                   .HasForeignKey(b => b.MemberId);
        }
    }
}
