using Interview_Calendar.Data;
using Interview_Calendar.Helpers;
using Interview_Calendar.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Configure Database Connection
builder.Services.Configure<UserDbConfiguration>(
        builder.Configuration.GetSection("InterviewCalendarDb")
    );



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configure mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Add Services and Helpers
//Add singleton to Injection
builder.Services.AddSingleton<IInterviewerService, InterviewerService>();
builder.Services.AddSingleton<PasswordHasher>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
