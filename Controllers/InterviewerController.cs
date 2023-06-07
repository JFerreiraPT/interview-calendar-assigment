using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;
using Interview_Calendar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using static Interview_Calendar.Models.RequiresClaimAttributes;

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
        //[Authorize]
        //[RequiresClaim(IdentityData.InterviewerUserClaim, "Interviewer")]
        public async Task<IActionResult> Create(UserCreateDTO userCreate)
        {
            var user = await _interviewerService.CreateUserAsync(userCreate);
            return CreatedAtAction(nameof(Create), new { nameof = user.Name }, user);
        }



        [HttpPost("{interviewerId}/availability")]
        [Authorize]
        [RequiresClaim(IdentityData.InterviewerUserPolicyName, "Interviewer")]
        public async Task<IActionResult> AddAvailability(string interviewerId, [FromBody] AvailabilityRequestDTO request)
        {
            if(await _interviewerService.AddAvailability(interviewerId, request.Date, request.TimeSlots)) {
                return Ok();
            }

            return new BadRequestResult();
        }

        [HttpDelete("{interviewerId}/availability")]
        [Authorize]
        [RequiresClaim(IdentityData.InterviewerUserPolicyName, "Interviewer")]
        public async Task<IActionResult> RemoveAvailability(string interviewerId, [FromBody] AvailabilityRequestDTO request)
        {
            if(await _interviewerService.RemoveDayAvailability(interviewerId, request.Date))
            {
                return Ok();
            }

            return new BadRequestResult();
        }


        [HttpGet("{interviewerId}/availability")]
        [Authorize]
        public IActionResult GetInterviewersWithSchedulesBetweenDates(string interviewerId, DateOnly startDate, DateOnly endDate)
        {
            // If no startDate is provided, set it to one day less than endDate or today's date
            if (startDate == DateOnly.MinValue)
            {
                startDate = endDate!= DateOnly.MinValue ?  endDate.AddDays(-1) : DateOnly.FromDateTime(DateTime.Today);
            }

            // If no endDate is provided, set it to one week after startDate or one week after today's date
            if (endDate == DateOnly.MinValue)
            {
                endDate = startDate != DateOnly.MinValue ? startDate.AddDays(7) : DateOnly.FromDateTime(DateTime.Today.AddDays(7));
            }


            Dictionary<string, SortedSet<int>> availability = _interviewerService.GetInterviewersWithSchedulesBetweenDates(interviewerId, startDate, endDate);

            return Ok(availability);
        }


    }
}

