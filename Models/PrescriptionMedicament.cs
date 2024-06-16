using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace APBD11Login.Models;


[PrimaryKey(nameof(IdMedicament), nameof(IdPrescription))]
public class PrescriptionMedicament
{
    public int IdMedicament { get; set; }
    public int IdPrescription { get; set; }
    public int Dose { get; set; }
    [MaxLength(100)] public string Details { get; set; } = null!;
    [ForeignKey(nameof(IdMedicament))] public Medicament Medicament { get; set; } = null!;
    [ForeignKey(nameof(IdPrescription))] public Prescription Prescription { get; set; } = null!;
}