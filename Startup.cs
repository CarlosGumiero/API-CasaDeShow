using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using APICasadeshow.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;

namespace APICasadeshow
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            string chaveDeSeguranca = "A_barata_da_vizinha_ta_na_minha_cama.";  //Chave de segurança
            var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSeguranca));
            // Usando o JWT como forme de autenticação
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x =>
            {
                //Como o sistgema vai ler o token
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "APICasaDeShow.com",
                    ValidAudience = "usuario",
                    IssuerSigningKey = chaveSimetrica
                };
            });

            services.AddControllersWithViews(x => x.AllowEmptyInputInBodyModelBinding = true);
            services.AddRazorPages();

            services.AddAuthorization(options => options.AddPolicy("Adm", policy => policy.RequireClaim("Adm", "True")));

            // Swagger
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "API DE CASA DE SHOW",
                    Version = "v1",
                    Description = "API aberta para acessar informações do projeto MVC",
                    // TermsOfService = new Uri("https://github.com/CarlosGumiero"),
                    Contact = new OpenApiContact
                    {
                        Name = "Carlos Gumiero GitHub",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/CarlosGumiero"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "LinkedIn",
                        Url = new Uri("https://www.linkedin.com/in/carlos-gumiero-011170161/"),
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); //Aplica o sistema de autenticação na aplicação
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            // app.UseSwagger(config => {
            //     config.RouteTemplate = "documentos/{documentname}/swagger.json";
            // }); // gera um arquivo json - Swagger.Json
            app.UseSwaggerUI(config => //Views HTML do Swagger
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "v1 docs");
                // config.RoutePrefix = string.Empty;
            });
        }
    }
}
