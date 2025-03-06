using appointmentApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.DataProtection;

namespace EcommerceApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Inicializando o builder da aplicação
            var builder = WebApplication.CreateBuilder(args);

            // Configuração de URL
            builder.WebHost.UseUrls("http://+:5000");

            // Carregar variáveis de ambiente
            Env.Load();
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            var dbName = Environment.GetEnvironmentVariable("DB");
            var jwtKey = Environment.GetEnvironmentVariable("Jwt__JWT_KEY");

            // Montando a connection string para o MySQL
            var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword}";

            // Configuração do DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Serviços básicos da API
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Injeção de dependências de Repositórios e Serviços
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configuração de proteção de dados (sem persistência em arquivos)
            builder.Services.AddDataProtection()
                .SetApplicationName("EcommerceApi");

            // Configuração do Identity para autenticação de usuários
            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddEntityFrameworkStores<AppDbContext>() // Adiciona suporte ao banco de dados
            .AddDefaultTokenProviders();

            // Configuração da autenticação JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    if (string.IsNullOrEmpty(jwtKey))
                    {
                        throw new ArgumentException("JWT key is null or empty");
                    }

                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Configuração de CORS para permitir acesso a todos os domínios
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Construa a aplicação
            var app = builder.Build();

            // Inicializar roles no banco de dados
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await RoleInitializer.SeedRolesAsync(roleManager);
            }

            // Configurar o pipeline HTTP
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configuração de middlewares
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            // Mapear os controllers
            app.MapControllers();

            // Rodar a aplicação
            app.Run();
        }
    }
}
