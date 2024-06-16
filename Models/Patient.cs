using System.ComponentModel.DataAnnotations;

namespace APBD11Login.Models;

public class Patient
{
    [Key] public int IdPatient { get; set; }
    [MaxLength(100)] public string FirstName { get; set; } = null!;
    [MaxLength(100)] public string LastName { get; set; } = null!;
    public DateTime Birthdate { get; set; }
    public ICollection<Prescription> Prescriptions { get; set; } = new HashSet<Prescription>();
}