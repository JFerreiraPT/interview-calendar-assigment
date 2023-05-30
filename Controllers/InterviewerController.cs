using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Services;
using Microsoft.AspNetCore.Mvc;

namespace Interview_Calendar.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class InterviewerController : ControllerBase
	{
        private readonly IInterviewerService _interviewerService;

        public InterviewerController(IInterviewerService interviewerService)
        {
            this._interviewerService = interviewerService;
        }


        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDTO userCreate)
        {
            var user = await _interviewerService.CreateUserAsync(userCreate);
            return CreatedAtAction(nameof(Create), new { nameof = user.Name }, user);
        }
    }
}

