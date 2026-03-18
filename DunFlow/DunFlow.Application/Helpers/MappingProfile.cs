using AutoMapper;
using DunFlow.Application.Contracts;
using DunFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DunFlow.Application.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WorkTask, WorkTaskDto>()
                .ForMember(dest => dest.WorkTaskTypeName, opt => opt.MapFrom(src => src.WorkTaskType != null ? src.WorkTaskType.Name : null));

            CreateMap<WorkTask, WorkTaskDetailsDto>()
                .ForMember(dest => dest.WorkTaskTypeName, opt => opt.MapFrom(src => src.WorkTaskType != null ? src.WorkTaskType.Name : null));

            CreateMap<CreateTaskRequest, WorkTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkTaskType, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsClosed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CustomFieldsJson, opt => opt.MapFrom(src => "{}"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<FormField, FormFieldDto>();

            CreateMap<WorkTaskStatus, WorkTaskStatusDto>();

            CreateMap<AppUser, UserDto>();
        }
    }
}
