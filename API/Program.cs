using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OnlineStoreWebAPI.DataContext;
using Microsoft.OpenApi.Models;
using LogicService;
using DataService;

var builder = WebApplication.CreateBuilder(args);

IConfiguration Configuration = builder.Configuration;

// **1. Add services to the container:**
builder.Services.AddControllers();

builder.Services.AddScoped<IUsersLogic, UsersLogic>();
builder.Services.AddScoped<IUsersData, UsersData>();

builder.Services.AddScoped<ICustomersLogic, CustomersLogic>();
builder.Services.AddScoped<ICustomersData, CustomersData>();

builder.Services.AddScoped<IProductsLogic, ProductsLogic>();
builder.Services.AddScoped<IProductsData, ProductsData>();

builder.Services.AddScoped<IOrdersLogic, OrdersLogic>();
builder.Services.AddScoped<IOrdersData, OrdersData>();

builder.Services.AddScoped<IAddressLogic, AddressLogic>();
builder.Services.AddScoped<IAddressData, AddressData>();

builder.Services.AddScoped<IReviewsLogic, ReviewsLogic>();
builder.Services.AddScoped<IReviewsData, ReviewsData>();

builder.Services.AddScoped<ICartLogic, CartLogic>();
builder.Services.AddScoped<ICartData, CartData>();

// **2. CorsSetup:**
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
           .AllowCredentials()
           .SetIsOriginAllowed((host) => true)
           .AllowAnyMethod()
           .AllowAnyHeader());
});

// **3. Swagger setup:**
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Online Store", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// **4. Entity Framework Core (Database Context Configuration)**
var connectionString = builder.Configuration.GetConnectionString("OnlineStoreDB");
builder.Services.AddDbContext<OnlineStoreDbContext>(options =>
    options.UseSqlServer(connectionString));

// **5. JWT Authentication Configuration**
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["Jwt:Issuer"],
        ValidAudience = Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? ""))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});

// **6. Enables authorization.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtAuth", policy =>
    {
        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
    });

    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });

    options.AddPolicy("UserOrAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin", "Customer");
    });
});

var app = builder.Build();

// **7. Configure the HTTP request pipeline:**

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy"); // Apply the CORS policy (optional).

app.UseAuthentication(); // Enable Authentication Middleware.

app.UseAuthorization();  // Enable Authorization Middleware.

app.MapControllers(); // Maps controllers to routes.

app.Run(); // Runs the application.
