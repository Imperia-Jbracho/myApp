using Microsoft.EntityFrameworkCore;
using MyApp.Models.Travel;

namespace MyApp.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Travel> Travels { get; set; }

        public DbSet<TravelParticipant> TravelParticipants { get; set; }

        public DbSet<TravelMilestone> TravelMilestones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Travel>(builder =>
            {
                builder.ToTable("Travels");
                builder.Property(travel => travel.Title)
                    .IsRequired()
                    .HasMaxLength(150);
                builder.Property(travel => travel.Destination)
                    .IsRequired()
                    .HasMaxLength(150);
                builder.Property(travel => travel.Currency)
                    .IsRequired()
                    .HasMaxLength(10);
                builder.Property(travel => travel.DurationDays)
                    .IsRequired();
                builder.Property(travel => travel.InitialBudget)
                    .HasColumnType("decimal(18,2)");
                builder.Property(travel => travel.StartDate)
                    .HasColumnType("date");
                builder.Property(travel => travel.EndDate)
                    .HasColumnType("date");
                builder.HasMany(travel => travel.Participants)
                    .WithOne(participant => participant.Travel)
                    .HasForeignKey(participant => participant.TravelId)
                    .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(travel => travel.Milestones)
                    .WithOne(milestone => milestone.Travel)
                    .HasForeignKey(milestone => milestone.TravelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TravelParticipant>(builder =>
            {
                builder.ToTable("TravelParticipants");
                builder.Property(participant => participant.Email)
                    .IsRequired()
                    .HasMaxLength(254);
                builder.Property(participant => participant.Role)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TravelMilestone>(builder =>
            {
                builder.ToTable("TravelMilestones");
                builder.Property(milestone => milestone.Type)
                    .IsRequired();
                builder.Property(milestone => milestone.Title)
                    .HasMaxLength(200);
                builder.Property(milestone => milestone.Date)
                    .HasColumnType("date");
                builder.Property(milestone => milestone.ReservationDate)
                    .HasColumnType("date");
                builder.Property(milestone => milestone.CheckInDate)
                    .HasColumnType("date");
                builder.Property(milestone => milestone.CheckOutDate)
                    .HasColumnType("date");
                builder.Property(milestone => milestone.StartTime)
                    .HasColumnType("time");
                builder.Property(milestone => milestone.EndTime)
                    .HasColumnType("time");
                builder.Property(milestone => milestone.Cost)
                    .HasColumnType("decimal(18,2)");
                builder.Property(milestone => milestone.NightlyRate)
                    .HasColumnType("decimal(18,2)");
                builder.Property(milestone => milestone.LocationUrl)
                    .HasMaxLength(500);
                builder.Property(milestone => milestone.Classification)
                    .HasMaxLength(200);
                builder.Property(milestone => milestone.BookingPlatform)
                    .HasMaxLength(150);
            });
        }
    }
}
