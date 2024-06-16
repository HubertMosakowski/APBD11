using System.ComponentModel.DataAnnotations;

namespace APBD11Login.DTOs;

public class PatientDTO
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime Birthdate { get; set; }
}