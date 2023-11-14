using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using TaskControl.Service.DTO;
using TaskControl.DAL;
using Microsoft.EntityFrameworkCore;

namespace TaskControl.Service
{
    public class Startup
    {
        public class ServiceMappingProfile : Profile
        {
            public ServiceMappingProfile()
            {
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
            services.AddAutoMapper(typeof(ServiceMappingProfile));
            services.AddDbContext<TaskDBContext>(opt => opt.UseSqlite(@"Data Source=C:/db/TaskControl/TaskControl.db"));
            services.AddScoped<ITaskService, TaskService>();
        }

    }
}
