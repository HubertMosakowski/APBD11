using APBD11Login.DTOs;

namespace APBD11Login.Services;

public interface IMedicamentsService
{
    public Task<PrescriptionDTO> AddPrescription(PrescriptionDTO prescriptionDto);
}