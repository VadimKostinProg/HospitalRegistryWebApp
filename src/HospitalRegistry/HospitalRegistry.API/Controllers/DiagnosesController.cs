﻿using HospitalRegistry.Application.Constants;
using HospitalRegistry.Application.DTO;
using HospitalRegistry.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalRegistry.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
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
            [FromQuery] DiagnosisSpecificationsDTO specifications)
        {
            return Ok(await _diagnosesService.GetDiagnosesListAsync(specifications));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<DiagnosisResponse>> GetDiagnosisById([FromRoute] Guid id)
        {
            return Ok(await _diagnosesService.GetByIdAsync(id));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<DiagnosisResponse>> CreateNewDiagnosis([FromBody] DiagnosisCreateRequest request)
        {
            return Ok(await _diagnosesService.CreateAsync(request));
        }

        [HttpPut]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<DiagnosisResponse>> UpdateDiagnosis([FromBody] DiagnosisUpdateRequest request)
        {
            return Ok(await _diagnosesService.UpdateAsync(request));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> DeleteDiagnosis([FromRoute] Guid id)
        {
            await _diagnosesService.DeleteAsync(id);

            return Ok($"Diagnosis \"{id}\" has been successfully deleted.");
        }

        [HttpGet("deleted")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<IEnumerable<DiagnosisResponse>>> GetDeletedDiagnosis(
            [FromQuery] DiagnosisSpecificationsDTO specifications)
        {
            return Ok(await _diagnosesService.GetDeletedDiagnosesListAsync(specifications));
        }

        [HttpPost("{id}/recover")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<string>> RecoverDiagnosis([FromRoute] Guid id)
        {
            await _diagnosesService.RecoverAsync(id);

            return Ok($"Diagnosis \"{id}\" has been successfully recovered.");
        }
    }
}