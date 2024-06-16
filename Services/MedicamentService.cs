using APBD11Login.Context;
using APBD11Login.DTOs;
using APBD11Login.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD11Login.Services;

public class MedicamentsService : IMedicamentsService
{
    private DbaseContext _dbaseContext;

    public MedicamentsService()
    {
        _dbaseContext = new DbaseContext();
    }

    public async Task<PrescriptionDTO> AddPrescription(PrescriptionDTO prescriptionDto)
    {
        Boolean patientExists = await _dbaseContext.Patients.AnyAsync(x => x.IdPatient == prescriptionDto.PatientDto.IdPatient);
        if (!patientExists)
        {
            Patient patient = new Patient()
            {
                Birthdate = prescriptionDto.PatientDto.Birthdate,
                FirstName = prescriptionDto.PatientDto.FirstName,
                LastName = prescriptionDto.PatientDto.LastName,
                Prescriptions = new List<Prescription>()
            };
            
            await _dbaseContext.AddAsync(patient);
            await _dbaseContext.SaveChangesAsync();
        }
        
        foreach (var med in prescriptionDto.Medicaments)
        {
            Boolean medExists = await _dbaseContext.Medicaments.FindAsync(med) == null;
            if (!medExists)
                throw new Exception("Medicament doesn't exist!");
        }

        int medsCount = prescriptionDto.Medicaments.Count;
        if (medsCount > 10)
            throw new Exception("Too many medicaments on prescription!");

        if (prescriptionDto.DueDate < prescriptionDto.Date)
            throw new Exception("Prescription expired!");

        Prescription prescription = new Prescription()
        {
            Date = prescriptionDto.Date,
            DueDate = prescriptionDto.DueDate,
            IdDoctor = prescriptionDto.Doctor.IdDoctor,
            IdPatient = prescriptionDto.PatientDto.IdPatient,
            IdPrescription = prescriptionDto.IdPrescription
        };

        await _dbaseContext.AddAsync(prescription);
        await _dbaseContext.SaveChangesAsync();

        foreach (var med in prescriptionDto.Medicaments)
        {
            PrescriptionMedicament preMed = new PrescriptionMedicament()
            {
                IdMedicament = med.IdMedicament,
                IdPrescription = prescriptionDto.IdPrescription,
                Dose = prescriptionDto.Dose,
                Details = prescriptionDto.Details
            };

            await _dbaseContext.AddAsync(preMed);
            await _dbaseContext.SaveChangesAsync();
        }

        return prescriptionDto;
    }
    
}
