using System;
using System.Net;
using Interview_Calendar.Exceptions;
using Interview_Calendar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Interview_Calendar.Filters
{
    public class InterviewCalendarExceptionFilter : IExceptionFilter
    {


        public void OnException(ExceptionContext context)
        {
            if (context.Exception is InvalidCredentialsException
                || context.Exception is ResourceExistsException
                || context.Exception is ValidationErrorException
                )
            {
                context.HttpContext.Response.StatusCode = 422;
                context.Result = new JsonResult(new Error{ StatusCode = 422, Message = context.Exception.Message });
            }
            else if (context.Exception is NotFoundException)
            {
                context.HttpContext.Response.StatusCode = 404;
                context.Result = new JsonResult(new Error { StatusCode = 404, Message = context.Exception.Message });
            }
            else
            {
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult(new Error { StatusCode = 500, Message = /*"Internal Server Error"*/context.Exception.Message });
            }


        }
    }
}

