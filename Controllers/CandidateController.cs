using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Services;
using Microsoft.AspNetCore.Mvc;

namespace Interview_Calendar.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class CandidateController : ControllerBase
	{
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDTO userCreate)
        {
            var user = await _candidateService.CreateUserAsync(userCreate);
            return CreatedAtAction(nameof(Create), new { nameof = user.Name }, user);
        }

        [HttpPatch("/{id}/interviewer")]
        public async Task<IActionResult> AssignInterviwer(string id, AddInterviewerDTO interviwer)
        {
            var user = await _candidateService.AssignInterviewer(id, interviwer);
            return Ok();
        }

        [HttpPost("/{id}/interview")]
        public async Task<IActionResult> AddInterview(string id, InterviewDTO interview)
        {
            var added = await _candidateService.ScheduleInterview(id, interview.date);
            return Ok();
        }

    }
}

