using System;
using Interview_Calendar.DTOs;
using Interview_Calendar.Models;

namespace Interview_Calendar.Services
{
    //T should be entity, DI should be input DTO and DO output DTO
	public interface IUserService<T, DI, DO >
        where T : User
        where DI : UserDTO
        where DO : UserDTO
	{
        T PreCreateUserAsync(DI dto);
        Task<DO> CreateUserAsync(DI dto);
        DO PostCreateUserAsync(T entity);
    }
}

