using APBD11Login.Services;
using APBD11Login.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APBD11Login.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    private IMedicamentsService _medicamentsService;

    public ApiController(IMedicamentsService medicamentsService)
    {
        _medicamentsService = medicamentsService;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription(PrescriptionDTO prescriptionDto)
    {
        PrescriptionDTO res = await _medicamentsService.AddPrescription(prescriptionDto);
        return Ok(res);
    }
}