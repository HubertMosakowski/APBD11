using APBD11Login.Models;
using APBD11Login.Models.AuthModels;
using Microsoft.EntityFrameworkCore;

namespace APBD11Login.Context;

public class DbaseContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
    
    public DbSet<AppUser> Users { get; set; }
    public DbaseContext()
    {
    }

    public DbaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>()
        {
            new() {IdMedicament = 1, Name = "l1", Description = "opis1", Type = "typ1"},
            new() {IdMedicament = 2, Name = "l2", Description = "opis2", Type = "typ2"},
            new() {IdMedicament = 3, Name = "l3", Description = "opis3", Type = "typ3"}
        });
    }
}