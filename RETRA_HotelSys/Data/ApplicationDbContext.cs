using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace RETRA_HotelSys.Data
{
    public class ApplicationDbContext : IdentityDbContext<HotelGuests>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StaffRoles> StaffRoles { get; set; }
        public DbSet<StaffMembers> StaffMembers { get; set; }
        public DbSet<HotelGuests> HotelGuests { get; set; }
        public DbSet<RoomCategories> RoomCategories { get; set; }
        public DbSet<HotelRooms> HotelRooms { get; set; }
        public DbSet<RoomFeatures> RoomFeatures { get; set; }
        public DbSet<GuestReservations> GuestReservations { get; set; }
        public DbSet<ReservationStatusHistory> ReservationStatusHistory { get; set; }
        public DbSet<ReservationPayments> ReservationPayments { get; set; }
        public DbSet<RoomMaintenance> RoomMaintenance { get; set; }
        public DbSet<RoomTypeFeatures> RoomTypeFeatures { get; set; }
        public object Staff { get; internal set; }
        public object Reservations { get; internal set; }
        public IEnumerable<object> MaintenanceTasks { get; internal set; }
        public object Rooms { get; internal set; }
        public IEnumerable<object> MaintenanceRequests { get; internal set; }
        public IEnumerable<object> Bookings { get; internal set; }
        public object RoomTypes { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal properties with precision and scale
            modelBuilder.Entity<GuestReservations>()
                .Property(gr => gr.DiscountAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<GuestReservations>()
                .Property(gr => gr.TaxAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<GuestReservations>()
                .Property(gr => gr.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<ReservationPayments>()
                .Property(rp => rp.Amount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<ReservationPayments>()
                .Property(rp => rp.RefundAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RoomCategories>()
                .Property(rc => rc.BasePrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RoomTypeFeatures>()
                .Property(rtf => rtf.AdditionalCost)
                .HasColumnType("decimal(18, 2)");

            // Configure composite primary key for RoomTypeFeatures
            modelBuilder.Entity<RoomTypeFeatures>()
                .HasKey(rtf => new { rtf.RoomTypeId, rtf.AmenityId });

            // Configure relationships for RoomTypeFeatures
            modelBuilder.Entity<RoomTypeFeatures>()
                .HasOne(rtf => rtf.RoomCategory)
                .WithMany(rc => rc.RoomTypeFeatures)
                .HasForeignKey(rtf => rtf.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomTypeFeatures>()
                .HasOne(rtf => rtf.RoomFeature)
                .WithMany(rf => rf.RoomTypeFeatures)
                .HasForeignKey(rtf => rtf.AmenityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StaffMembers to StaffRoles relationship
            modelBuilder.Entity<StaffMembers>()
                .HasOne(sm => sm.StaffRole)
                .WithMany(sr => sr.StaffMembers)
                .HasForeignKey(sm => sm.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure HotelRooms to RoomCategories relationship
            modelBuilder.Entity<HotelRooms>()
                .HasOne(hr => hr.RoomCategory)
                .WithMany(rc => rc.HotelRooms)
                .HasForeignKey(hr => hr.RoomTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure GuestReservations relationships
            modelBuilder.Entity<GuestReservations>()
                .HasOne(gr => gr.Guest)
                .WithMany(hg => hg.GuestReservations)
                .HasForeignKey(gr => gr.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GuestReservations>()
                .HasOne(gr => gr.Room)
                .WithMany(hr => hr.GuestReservations)
                .HasForeignKey(gr => gr.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReservationStatusHistory relationships
            modelBuilder.Entity<ReservationStatusHistory>()
                .HasOne(rsh => rsh.Reservation)
                .WithMany(gr => gr.StatusHistory)
                .HasForeignKey(rsh => rsh.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationStatusHistory>()
                .HasOne(rsh => rsh.ChangedByStaff)
                .WithMany()
                .HasForeignKey(rsh => rsh.ChangedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // ReservationPayments relationships
            modelBuilder.Entity<ReservationPayments>()
                .HasOne(rp => rp.Reservation)
                .WithMany(gr => gr.Payments)
                .HasForeignKey(rp => rp.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationPayments>()
                .HasOne(rp => rp.ProcessedByStaff)
                .WithMany()
                .HasForeignKey(rp => rp.ProcessedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure RoomMaintenance relationships
            modelBuilder.Entity<RoomMaintenance>()
                .HasOne(rm => rm.HotelRoom)
                .WithMany(hr => hr.RoomMaintenances)
                .HasForeignKey(rm => rm.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomMaintenance>()
                .HasOne(rm => rm.AssignedStaff)
                .WithMany()
                .HasForeignKey(rm => rm.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomMaintenance>()
                .HasOne(rm => rm.CreatedByStaff)
                .WithMany()
                .HasForeignKey(rm => rm.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StaffRoles primary key
            modelBuilder.Entity<StaffRoles>()
                .HasKey(sr => sr.RoleId);

            modelBuilder.Entity<StaffMembers>(entity =>
            {
                entity.HasIndex(s => s.Username).IsUnique();
                entity.HasIndex(s => s.Email).IsUnique();
                entity.Property(s => s.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<RoomFeatures>(entity =>
            {
                entity.Property(e => e.DefaultAdditionalCost)
                      .HasColumnType("decimal(18,2)");
                // OR: .HasPrecision(18, 2); // .NET 6+
            });

            modelBuilder.Entity<RoomFeatures>()
               .Property(e => e.DefaultAdditionalCost)
               .HasPrecision(18, 2);

            // Configure GuestReservations relationships
            modelBuilder.Entity<GuestReservations>()
                .HasOne(gr => gr.Guest)
                .WithMany(hg => hg.GuestReservations)
                .HasForeignKey(gr => gr.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GuestReservations>()
                .HasOne(gr => gr.Room)
                .WithMany(hr => hr.GuestReservations)
                .HasForeignKey(gr => gr.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReservationStatusHistory relationships
            modelBuilder.Entity<ReservationStatusHistory>()
                .HasOne(rsh => rsh.Reservation)
                .WithMany(gr => gr.StatusHistory)
                .HasForeignKey(rsh => rsh.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationStatusHistory>()
                .HasOne(rsh => rsh.ChangedByStaff)
                .WithMany()
                .HasForeignKey(rsh => rsh.ChangedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // ReservationPayments relationships
            modelBuilder.Entity<ReservationPayments>()
                .HasOne(rp => rp.Reservation)
                .WithMany(gr => gr.Payments)
                .HasForeignKey(rp => rp.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationPayments>()
                .HasOne(rp => rp.ProcessedByStaff)
                .WithMany()
                .HasForeignKey(rp => rp.ProcessedBy)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GuestReservations>(entity =>
            {
                entity.Property(e => e.GuestEmail)
                    .HasMaxLength(256); // Adjust max length as needed

                entity.Property(e => e.GuestPhone)
                    .HasMaxLength(20); // Adjust max length as needed
            });


        }
    }
}