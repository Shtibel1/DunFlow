using DunFlow.Domain.Enums;
using DunFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.Interfaces
{
    public interface ITaskTypeStrategy
    {
        TaskType TaskType { get; }
        int FinalStatus { get; }
        bool ValidateRequiredData(int targetStatus, string customFieldsJson, out string errorMessage);
        IEnumerable<FormFieldDefinition> GetFormSchema(int targetStatus);
        IDictionary<int, string> GetStatusNames();
    }
}
