using DunFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Interfaces
{
    public interface ITaskStrategyResolver
    {
        ITaskTypeStrategy GetStrategy(TaskType type);
        IEnumerable<TaskType> GetSupportedTaskTypes();
        IEnumerable<ITaskTypeStrategy> GetAll();
    }
}
