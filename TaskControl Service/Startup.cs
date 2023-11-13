
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TaskControl.Service;
using TaskControl.Service.DTO;

namespace TaskControl.Service
{
    public class Startup
    {
        public class ServiceMappingProfile : Profile
        {
            public ServiceMappingProfile()
            {
                CreateMap<TaskDto, DAL.Entity.Task>();
                CreateMap<DAL.Entity.Task, TaskDto>();
                CreateMap<List<DAL.Entity.Task>, List<TaskDto>>();
            }
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ServiceMappingProfile));
            services.AddScoped<ITaskService, TaskService>();
        }

    }
}
