using AutoMapper;
using DunFlow.Application.Contracts;
using DunFlow.Domain.Entities;
using DunFlow.Domain.ValueObjects;
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
            CreateMap<WorkTask, WorkTaskDto>();

            CreateMap<WorkTask, WorkTaskDetailsDto>();

            CreateMap<CreateTaskRequest, WorkTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.IsClosed, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CustomFieldsJson, opt => opt.MapFrom(src => "{}"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<FormFieldDefinition, FormFieldDto>();

            CreateMap<AppUser, UserDto>();
        }
    }
}
