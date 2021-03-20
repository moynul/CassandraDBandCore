using Cassandra;
using CassandraCore.Models;
using CassandraCore.Models.Repos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CassandraCore
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
            services.AddControllersWithViews();
            services.AddScoped<IStudentRepository, StudentRepository>();

            #region Cassandra
            var keySpace = Configuration["KeySpace"];
            var table = Configuration["Table"];
            var address = Configuration["Address"];

            using (Cluster cluster = Cluster.Builder()
                .AddContactPoint(address)
                .WithDefaultKeyspace("main")
                .Build())
            using (ISession session = cluster.ConnectAndCreateDefaultKeyspaceIfNotExists())
            {
                session.Execute(
                    $"create keyspace if not exists {keySpace} with replication ={{'class':'SimpleStrategy','replication_factor':3}};");

                session.Execute($"use {keySpace}");

                session.Execute(
                    $"create table if not exists {table}(id uuid primary key, name text, address text );");
            }

            services.AddScoped<ICluster>(s => Cluster.Builder().AddContactPoint(address)
                .Build());

            services.AddScoped<ISession>(s => s.GetService<ICluster>().Connect(keySpace));

            #endregion


            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Cassandra core",
                });

            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PatientAppointmentService");
                c.RoutePrefix = string.Empty;
            });
            #endregion
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
