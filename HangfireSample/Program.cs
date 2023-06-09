using Hangfire;
using Hangfire.SqlServer;
using HangfireSample.Job;
using HangfireSample.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHangfire(configuration => configuration
       .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
       .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
       {
           CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
           SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
           QueuePollInterval = TimeSpan.Zero,
           UseRecommendedIsolationLevel = true,
           DisableGlobalLocks = true
       }));

builder.Services.AddHangfireServer();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    DashboardTitle = "Hangfire Dashboard",
    Authorization = new[]{
    new HangfireBasicAuthenticationFilter.HangfireCustomBasicAuthenticationFilter{
        User = builder.Configuration.GetSection("HangfireCredentials:UserName").Value,
        Pass = builder.Configuration.GetSection("HangfireCredentials:Password").Value
    }}
});

RecurringJob.AddOrUpdate<CargoJob>("SendCargo", x => x.SendToCargo(), "*/10 * * * *", TimeZoneInfo.Local);
RecurringJob.AddOrUpdate<CargoJob>("UpdateCargoStatus", x => x.UpdateCargoStatus(), "*/5 * * * *", TimeZoneInfo.Local);

app.MapControllers();

app.Run();
