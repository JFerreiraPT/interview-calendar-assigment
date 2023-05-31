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



        [HttpPost("{interviewerId}/availability")]
        public async Task<IActionResult> AddAvailability(string interviewerId, [FromBody] AvailabilityRequestDTO request)
        {
            try
            {
                await _interviewerService.AddAvailability(interviewerId, request.Date, request.TimeSlots);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{interviewerId}/availability")]
        public async Task<IActionResult> RemoveAvailability(string interviewerId, [FromBody] AvailabilityRequestDTO request)
        {
            try
            {
                await _interviewerService.RemoveDayAvailability(interviewerId, request.Date);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

       
    }
}

