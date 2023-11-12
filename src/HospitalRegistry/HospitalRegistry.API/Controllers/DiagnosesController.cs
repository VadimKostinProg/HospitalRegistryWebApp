using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRegistry.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class DiagnosesController : ControllerBase
    {
        private readonly IDiagnosesService _diagnosesService;

        public DiagnosesController(IDiagnosesService diagnosesService)
        {
            _diagnosesService = diagnosesService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DiagnosisResponse>>> GetDiagnosisList(
            [FromQuery] Specifications specifications)
        {
            return Ok(await _diagnosesService.GetAllAsync(specifications));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DiagnosisResponse>> GetDiagnosisById([FromRoute] Guid id)
        {
            return Ok(await _diagnosesService.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<DiagnosisResponse>> CreateNewDiagnosis([FromBody] DiagnosisAddRequest request)
        {
            return Ok(await _diagnosesService.CreateAsync(request));
        }

        [HttpPut]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> UpdateDiagnosis([FromBody] DiagnosisUpdateRequest request)
        {
            return Ok(await _diagnosesService.UpdateAsync(request));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> DeleteDiagnosis([FromBody] Guid id)
        {
            await _diagnosesService.DeleteAsync(id);

            return Ok($"Diagnosis \"{id}\" has been successfully deleted.");
        }
    }
}