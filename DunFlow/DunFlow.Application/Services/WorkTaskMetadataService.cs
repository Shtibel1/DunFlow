using AutoMapper;
using DunFlow.Application.Contracts;
using DunFlow.Application.Interfaces;
using DunFlow.Domain.Entities;
using DunFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application.Services
{
    public class WorkTaskMetadataService : IWorkTaskMetadataService
    {
        private readonly IWorkTaskMetadataRepository _metadataRepository;
        private readonly IMapper _mapper;

        public WorkTaskMetadataService(IWorkTaskMetadataRepository metadataRepository, IMapper mapper)
        {
            _metadataRepository = metadataRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskTypeDto>> GetTaskTypeMetadataAsync()
        {
            var types = await _metadataRepository.GetAllTypesWithStatusesAsync();

            return types.Select(t => new TaskTypeDto
            {
                TypeValue = t.Id,
                TaskType = t.Name,
                FinalStatus = t.Statuses.FirstOrDefault(s => s.IsFinal)?.StatusValue ?? 0,
                Statuses = _mapper.Map<List<WorkTaskStatusDto>>(t.Statuses.ToList())
            });
        }

        public async Task<IEnumerable<FormFieldDto>> GetFormSchemaAsync(int workTaskTypeId, int targetStatus)
        {
            var fields = await _metadataRepository.GetFormFieldsByStatusValueAsync(workTaskTypeId, targetStatus);

            if (!fields.Any())
            {
                return [];
            }

            return _mapper.Map<IEnumerable<FormFieldDto>>(fields);
        }

        public async Task<WorkTaskStatus?> GetStatusEntityAsync(int workTaskTypeId, int statusValue)
        {
            return await _metadataRepository.GetStatusAsync(workTaskTypeId, statusValue);
        }

        public async Task<IEnumerable<FormField>> GetRawFormFieldsAsync(int statusId)
        {
            return await _metadataRepository.GetFormFieldsAsync(statusId);
        }
    }
}
