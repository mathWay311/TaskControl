using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using TaskControl.Service;
using TaskControl.Models;
using TaskControl.Service.DTO;

namespace TaskControl
{
    public class Startup
    {
        public class AppMappingProfile : Profile
        {
            public AppMappingProfile()
            {
                CreateMap<TaskViewModel, TaskDto>().ReverseMap();
                CreateMap<Models.TaskStatus, Service.DTO.TaskStatus>().ReverseMap();
                CreateMap<TaskDto, DAL.Entity.Task>().ReverseMap();
            }
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(AppMappingProfile));
            services.AddScoped<ITaskService, TaskService> ();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
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
