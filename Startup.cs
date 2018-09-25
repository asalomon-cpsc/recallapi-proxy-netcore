using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opendata.Recalls.Repository;

namespace recallMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder => 
                    builder.WithOrigins("http://localhost:8082", "http://vmdevdesk13:8082")
                           .AllowAnyMethod()
                           .AllowCredentials()
                           .AllowAnyHeader()
                           
                    );

            });
            services.AddMvc();
            services.AddScoped<IRecallApiProxyRepository, RecallApiProxyRepository>();
            services.AddScoped<IStatsLogger, StatsLogger>();
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                

            }

              app.UseCors("AllowSpecificOrigin");



            app.UseMvc();
        }
    }
}
