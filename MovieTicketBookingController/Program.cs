using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.Application.Interfaces;
using MovieTicketBooking.Infrastructure.Configuration;
using MovieTicketBooking.Infrastructure.Repositories;
using MovieTicketBooking.Infrastructure.Services;
using MovieTicketBookingApplication.Interfaces;
using MovieTicketBookingApplication.Interfaces.Movie;
using MovieTicketBookingApplication.Interfaces.Movies;
using MovieTicketBookingApplication.Interfaces.Screen;
using MovieTicketBookingDomain.Settings;
using MovieTicketBookingInfrastructure.Repository.Screen;
using MovieTicketBookingInfrastructure.Service;
using MovieTicketBookingInfrastructure.Service.Movies;
using MovieTicketBookingInfrastructure.Service.Screen;
using MovieTicketBookingInfrastructure.Service.TheaterAdmin;
using MovieTicketBookingInfrastructureData;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Register SQL Connection Factory
builder.Services.AddSingleton<SqlConnectionFactory>();
// Register JwtSetting
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));


// Register Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddScoped<ITheaterRepository, TheaterRepository>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<MovieTicketBookingApplication.Interfaces.Show.IShowRepository, MovieTicketBookingInfrastructure.Repository.Show.ShowRepository>();


// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ITheaterService, TheaterService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
// Register new modules
builder.Services.AddScoped<IMasterDataRepository, MasterDataRepository>();
builder.Services.AddScoped<IMasterDataService, MasterDataService>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.SuperAdmin.Dashboard.ISuperAdminDashboardRepository, MovieTicketBooking.Infrastructure.Repository.SuperAdmin.Dashboard.SuperAdminDashboardRepository>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.SuperAdmin.Dashboard.ISuperAdminDashboardService, MovieTicketBooking.Infrastructure.Service.SuperAdmin.Dashboard.SuperAdminDashboardService>();
builder.Services.AddScoped<MovieTicketBookingApplication.Interfaces.Show.IShowService, MovieTicketBookingInfrastructure.Service.Show.ShowService>();

// Register Customer Modules
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Customer.ICustomerMovieRepository, MovieTicketBooking.Infrastructure.Repository.Customer.CustomerMovieRepository>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Customer.ICustomerMovieService, MovieTicketBooking.Infrastructure.Service.Customer.CustomerMovieService>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Customer.ICustomerTheaterRepository, MovieTicketBooking.Infrastructure.Repository.Customer.CustomerTheaterRepository>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Customer.ICustomerTheaterService, MovieTicketBooking.Infrastructure.Service.Customer.CustomerTheaterService>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Customer.ICustomerShowRepository, MovieTicketBooking.Infrastructure.Repository.Customer.CustomerShowRepository>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Customer.ICustomerShowService, MovieTicketBooking.Infrastructure.Service.Customer.CustomerShowService>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Booking.IBookingRepository, MovieTicketBooking.Infrastructure.Repository.Booking.BookingRepository>();
builder.Services.AddScoped<MovieTicketBooking.Application.Interfaces.Booking.IBookingService, MovieTicketBooking.Infrastructure.Service.Booking.BookingService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<OtpGenerator>();

builder.Services.AddHttpContextAccessor();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;


builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,

            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["MovieTicketBookingJWT"];
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var requestPath = context.Request.Path;
                if (!requestPath.StartsWithSegments("/api"))
                {
                    context.HandleResponse();
                    var returnUrl = Uri.EscapeDataString(requestPath + context.Request.QueryString);
                    context.Response.Redirect($"/Account/Login?ReturnUrl={returnUrl}");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Configure HTTP Request Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();