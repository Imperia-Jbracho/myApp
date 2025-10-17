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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Travel>(builder =>
            {
                builder.ToTable("Travels");
                builder.Property(travel => travel.Title)
                    .IsRequired()
                    .HasMaxLength(150);
                builder.Property(travel => travel.Currency)
                    .IsRequired()
                    .HasMaxLength(10);
                builder.Property(travel => travel.DurationDays)
                    .IsRequired();
                builder.Property(travel => travel.StartDate)
                    .HasColumnType("date");
                builder.Property(travel => travel.EndDate)
                    .HasColumnType("date");
                builder.HasMany(travel => travel.Participants)
                    .WithOne(participant => participant.Travel)
                    .HasForeignKey(participant => participant.TravelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TravelParticipant>(builder =>
            {
                builder.ToTable("TravelParticipants");
                builder.Property(participant => participant.Email)
                    .IsRequired()
                    .HasMaxLength(254);
            });
        }
    }
}
