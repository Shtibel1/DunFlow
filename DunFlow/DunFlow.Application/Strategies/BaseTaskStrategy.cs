using DunFlow.Domain.Enums;
using DunFlow.Domain.Interfaces;
using DunFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DunFlow.Application.Strategies
{
    public abstract class BaseTaskStrategy<TEnum> : ITaskTypeStrategy where TEnum : Enum
    {
        public abstract TaskType TaskType { get; }
        public abstract int FinalStatus { get; }
        public abstract IEnumerable<FormFieldDefinition> GetFormSchema(int targetStatus);

        public IDictionary<int, string> GetStatusNames()
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .ToDictionary(
                           x => Convert.ToInt32(x), 
                           x => GetDescription(x)
                       );
        }

        private string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString(); 
        }

        public virtual bool ValidateRequiredData(int targetStatus, string customFieldsJson, out string errorMessage)
        {
            errorMessage = string.Empty;
            var schema = GetFormSchema(targetStatus);

            if (!schema.Any(f => f.IsRequired)) return true;

            var json = string.IsNullOrWhiteSpace(customFieldsJson) ? "{}" : customFieldsJson;
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            foreach (var field in schema.Where(f => f.IsRequired))
            {
                if (!root.TryGetProperty(field.Key, out var property) || string.IsNullOrWhiteSpace(property.ToString()))
                {
                    var statusName = Enum.GetName(typeof(TEnum), targetStatus);
                    errorMessage = $"{field.Label} is required for status {statusName}.";
                    return false;
                }
            }

            return true;
        }
    }
}
