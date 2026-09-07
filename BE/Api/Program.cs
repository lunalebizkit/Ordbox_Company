using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ordbox.Domain;
using Ordbox.SDK.Error;
using Ordbox.Services.ARCA;
using Ordbox.Services.ARCA.Dto;
using Ordbox.Services.ARCA.Interface;
using Ordbox.Services.ImpresoraFiscal;
using Ordbox.Services.ImpresoraFiscal.Printer250F;
using Ordbox.Services.ImpresoraFiscal.PrinterF250F;
using Ordbox.Services.Mapper;
using Ordbox.Services.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("http://0.0.0.0:80");

var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.AddSerilog(logger);
var connectionString = builder.Configuration.GetConnectionString("sqlconnection");

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

if (allowedOrigins == null || allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("CORS configuration error: AllowedOrigins is empty or missing.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", builder =>
    builder.WithOrigins(allowedOrigins)
           .AllowAnyMethod()
           .AllowAnyHeader());
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordbox.Api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

builder.Services.AddSingleton<IPrinter, PrinterF250F>();
builder.Services.AddSingleton<PrinterStatus>(p => builder.Configuration.GetSection("PrinterStatus").Get<PrinterStatus>());
builder.Services.AddSingleton<PrinterConfig>(p => builder.Configuration.GetSection("PrinterConfig").Get<PrinterConfig>());
builder.Services.AddSingleton<ArcaConfig>(p => builder.Configuration.GetSection("ArcaConfig").Get<ArcaConfig>());
builder.Services.AddScoped<IArcaIntegracion, ArcaIntegracionService>();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<UserMapperProfile>();
    cfg.AddProfile<AlicuotaIvaMapperProfile>();
    cfg.AddProfile<BrandMapperProfile>();
    cfg.AddProfile<BudgetMapperProfile>();
    cfg.AddProfile<CategoryMapperProfile>();
    cfg.AddProfile<CreditMemoMapper>();
    cfg.AddProfile<DebitMemoMapperProfile>();
    cfg.AddProfile<DeliveryNotesMapperProfile>();
    cfg.AddProfile<EntityMapperProfile>();
    cfg.AddProfile<IntegrationLogMapperProfile>();
    cfg.AddProfile<InvoiceMapperProfile>();
    cfg.AddProfile<IvaDigitalMapperProfile>();
    cfg.AddProfile<IvaMapperProfile>();
    cfg.AddProfile<OrderSupplierMapperProfile>();
    cfg.AddProfile<PeriodMapperProfile>();
    cfg.AddProfile<ProductMapperProfile>();
    cfg.AddProfile<QuittanceMapperProfile>();
    cfg.AddProfile<ReceiptMapperProfile>();
    cfg.AddProfile<RolMapperProfile>();
    cfg.AddProfile<CompanyMapperProfile>();
});
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddSingleton<ErrorManager>();
builder.Services.AddScoped<InvoiceService>();
builder.Services.AddScoped<EntityService>();
builder.Services.AddScoped<SupplierOrderService>();
builder.Services.AddScoped<ReceiptService>();
builder.Services.AddScoped<PeriodService>();
builder.Services.AddScoped<IvaService>();
builder.Services.AddScoped<DebitMemoService>();
builder.Services.AddScoped<CreditMemoService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ReporteZService>();
builder.Services.AddScoped<DeliveryNotesService>();
builder.Services.AddScoped<BudgetService>();
builder.Services.AddScoped<QuittanceService>();
builder.Services.AddDbContext<DBContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddScoped<ReimprimirDocService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<CompanyService>();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
     };
 });

var app = builder.Build();
app.UseRouting();
app.UseCors("AllowAngularClient");
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

