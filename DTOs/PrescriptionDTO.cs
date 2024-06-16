using APBD11Login.DTOs;
using APBD11Login.Models;

namespace APBD11Login.DTOs;

public class PrescriptionDTO
{
    public PatientDTO PatientDto { get; set; } = null!;
    public ICollection<Medicament> Medicaments { get; set; } = new List<Medicament>();
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public Doctor Doctor { get; set; } = null!;
    public int Dose { get; set; }
    public string Details { get; set; } = null!;
    public int IdPrescription { get; set; }
}