using TireSearchMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add API controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add HttpClient
builder.Services.AddHttpClient();

// Add TireService
builder.Services.AddScoped<ITireService, TireService>();
// Add BrandService
builder.Services.AddScoped<IBrandService, BrandService>();
// Add VehicleGroupService
builder.Services.AddScoped<IVehicleGroupService, VehicleGroupService>();
// Add VehicleModelService
builder.Services.AddScoped<IVehicleModelService, VehicleModelService>();
// Add VehicleVersionService
builder.Services.AddScoped<IVehicleVersionService, VehicleVersionService>();
// Add BrandCodeDescriptionService
builder.Services.AddScoped<IBrandCodeDescriptionService, BrandCodeDescriptionService>();
// Add ParentGroupService
builder.Services.AddScoped<IParentGroupService, ParentGroupService>();
// Add SeasonService
builder.Services.AddScoped<ISeasonService, SeasonService>();
// Add SegmentService
builder.Services.AddScoped<ISegmentService, SegmentService>();
// Add TsSegmentService
builder.Services.AddScoped<ITsSegmentService, TsSegmentService>();
// Add SpecialUsageService
builder.Services.AddScoped<ISpecialUsageService, SpecialUsageService>();
// Add DailyStatsService
builder.Services.AddScoped<IDailyStatsService, DailyStatsService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

// Map MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SizeVariant}/{action=Index}/{id?}");

// Map API routes
app.MapControllers();

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run(); 