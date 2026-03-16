using DunFlow.Domain.Enums;
using DunFlow.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Application
{
    public class TaskStrategyResolver : ITaskStrategyResolver
    {
        private readonly IReadOnlyDictionary<TaskType, ITaskTypeStrategy> _strategies;

        public TaskStrategyResolver(IEnumerable<ITaskTypeStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.TaskType, s => s);
        }

        public ITaskTypeStrategy GetStrategy(TaskType type)
        {
            if (!_strategies.TryGetValue(type, out var strategy))
            {
                throw new NotSupportedException($"Strategy for task type {type} is not implemented.");
            }

            return strategy;
        }

        public IEnumerable<ITaskTypeStrategy> GetAll()
        {
            return _strategies.Values;
        }

        public IEnumerable<TaskType> GetSupportedTaskTypes()
        {
            return _strategies.Keys;
        }
    }
}
