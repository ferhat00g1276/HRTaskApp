
using Entity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<EmployeeDepartment> EmployeeDepartments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Em
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.FirstName).IsUnique();
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Salary).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.Balance).HasDefaultValue(0);
            });

            // Department configuration
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
                entity.Property(d => d.AdditionalInfo).HasMaxLength(500);
            });

            // EmployeeDepartment (junction table) configuration
            modelBuilder.Entity<EmployeeDepartment>(entity =>
            {
                entity.HasKey(ed => new { ed.EmployeeId, ed.DepartmentId });

                entity.HasOne(ed => ed.Employee)
                      .WithMany(e => e.EmployeeDepartments)
                      .HasForeignKey(ed => ed.EmployeeId);

                entity.HasOne(ed => ed.Department)
                      .WithMany(d => d.EmployeeDepartments)
                      .HasForeignKey(ed => ed.DepartmentId);
            });
        }
    }
}


