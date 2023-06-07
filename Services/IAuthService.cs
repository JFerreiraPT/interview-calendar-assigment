using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Services
{
	public interface IAuthService
	{

        string GenerateToken(User user);
        LoginResponseDTO Login(LoginDTO loginDTO);

    }
}

