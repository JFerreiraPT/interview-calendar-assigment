using System;
namespace Interview_Calendar.DTOs
{
	public class LoginResponseDTO
	{
		public string Id { get; set; } = default!;
		public string Token { get; set; } = default!;
		public string Name { get; set; } = default!;
		public string Email { get; set; } = default!;

	}
}

