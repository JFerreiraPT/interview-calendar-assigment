using Interview_Calendar.Data;
using Interview_Calendar.Helpers;
using Interview_Calendar.Models;
using Interview_Calendar.Services;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
});

builder.Services.AddControllers().AddNewtonsoftJson();

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
builder.Services.AddSingleton<ICandidateService, CandidateService>();
builder.Services.AddSingleton<PasswordHasher>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//We need this to serialize Availability property in order to be able to seacrh for it
BsonClassMap.RegisterClassMap<Interviewer>(cm =>
{
    cm.AutoMap();
var memberMap = cm.GetMemberMap(x => x.Availability);
var serializer = memberMap.GetSerializer();
if (serializer is IDictionaryRepresentationConfigurable dictionaryRepresentationSerializer)
    serializer = dictionaryRepresentationSerializer.WithDictionaryRepresentation(DictionaryRepresentation.ArrayOfDocuments);
memberMap.SetSerializer(serializer);
});



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
