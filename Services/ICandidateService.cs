using System;
using System.Threading.Tasks;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Services
{
    public interface ICandidateService : IUserService<Candidate, UserCreateDTO, CandidateResponseDTO>
    {
        Task<CandidateResponseDTO> GetCandidate(string candidateId);
        Task<bool> AssignInterviewer(string id, AddInterviewerDTO interviwer);
        Task<bool> ScheduleInterview(string candidateId, DateTime date);

        Candidate FindOrFail(string candidateId);
    }
}

