using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using challenge.Data;
using Microsoft.EntityFrameworkCore;
using challenge.Repositories;
using challenge.Services;
using challenge.Controllers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace code_challenge.Tests.Integration
{
    public class TestServerStartup
    {
        public TestServerStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EmployeeContext>(options =>
            {
                options.UseInMemoryDatabase("EmployeeDB");
            });
            services.AddDbContext<CompensationContext>(options =>
            {
                options.UseInMemoryDatabase("CompensationDB");
            });
            services.AddTransient<EmployeeDataSeeder>();
            services.AddTransient<CompensationDataSeeder>();
            services.AddScoped<IEmployeeRepository,EmployeeRespository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICompensationRepository, CompensationRespository>();
            services.AddScoped<ICompensationService, CompensationService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, EmployeeDataSeeder eseeder, CompensationDataSeeder cseeder)
        {
            if (env.IsDevelopment())
            {
                var eseed = eseeder.Seed();
                var cseed = cseeder.Seed();
                
                app.UseDeveloperExceptionPage();
                Task.WaitAll(eseed, cseed);
            }

            app.UseMvc();
        }
    }
}
