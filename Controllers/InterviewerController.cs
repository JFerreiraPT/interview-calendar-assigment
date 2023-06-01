﻿using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;
using Interview_Calendar.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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


        [HttpGet("{interviewerId}/availability")]
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

