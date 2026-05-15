using AbcPharmacy.Middleware;
using AbcPharmacy.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // camelCase JSON responses
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        // Serialize enums as strings
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ── Swagger / OpenAPI ─────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "ABC Pharmacy API",
        Version     = "v1",
        Description = "REST API for managing medicines and sale records at ABC Pharmacy."
    });
    // Include XML comments (optional — generated from /// summaries)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ── In-memory Data Store (Singleton — shared state across requests) ──
builder.Services.AddSingleton<DataStore>();

// ── Application Services ──────────────────────────────────────────
builder.Services.AddScoped<IMedicineService, MedicineService>();
builder.Services.AddScoped<ISaleService, SaleService>();

// ── CORS — allow frontend during development ──────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ── Logging ───────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────

// 1. Global exception handler — must be first
app.UseMiddleware<GlobalExceptionMiddleware>();

// 2. Swagger UI (all environments for now — restrict to Dev in production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ABC Pharmacy API v1");
    c.RoutePrefix    = string.Empty;          // Swagger at root: http://localhost:5000
    c.DocumentTitle  = "ABC Pharmacy API";
});

// 3. HTTPS redirect
app.UseHttpsRedirection();

// 4. CORS
app.UseCors("AllowAll");

// 5. Routing + controllers
app.UseRouting();
app.MapControllers();

app.Run();
