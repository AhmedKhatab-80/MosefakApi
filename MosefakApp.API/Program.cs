using Hangfire;
using MosefakApi.Business.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.OptionsPatternConfig(builder.Configuration); // belong IOptions Pattern

builder.Services.AddDataProtection();
builder.Services.AddMemoryCache();

// ✅ Configure Serilog from `appsettings.json`
builder.Host.UseCustomSerilog();

// Call Container here

builder.Services.RegisterConfiguration(builder.Configuration);
builder.Services.RegisterIdentityConfig();
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterFluentValidationSettings();

// for permission based authorization

builder.Services.AddTransient(typeof(IAuthorizationHandler), typeof(PermissionAuthorizationHandler));
builder.Services.AddTransient(typeof(IAuthorizationPolicyProvider), typeof(PermissionAuthorizationPolicyProvider));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerServices();

// Call Seed Data

//await builder.Services.Seeding();


builder.Services.AddAuthentication(builder.Configuration);

#region For Validation Error

builder.Services.Configure();

#endregion
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Run recurring job
using (var scope = app.Services.CreateScope())
{
    var appointmentService = scope.ServiceProvider.GetRequiredService<AppointmentService>();

    string recurringJobId = "activateEmployeesJob";
    // Using service-based API for recurring job
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate(
        recurringJobId,
        () => appointmentService.AutoCancelUnpaidAppointments(),
        Cron.Daily
    );
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseStaticFiles();  // it's very very Important after added wwwroot folder and folder of images that belong each entity. 

// ✅ Log every request in a structured way
app.UseSerilogRequestLogging();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<CalculateTimeOfRequest>();
app.UseMiddleware<ErrorHandlingMiddleWare>();


app.MapControllers();
app.Run();
