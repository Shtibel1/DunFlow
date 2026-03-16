using DunFlow.Domain.Enums;
using DunFlow.Domain.Interfaces;
using DunFlow.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DunFlow.Application.Strategies
{
    public class DevelopmentStrategy : BaseTaskStrategy<DevelopmentStrategy.DevelopmentStatus>
    {
        public override TaskType TaskType => TaskType.Development;
        public override int FinalStatus => 4;

        public enum DevelopmentStatus
        {
            [Description("Created")] Created = 1,
            [Description("Specification Completed")] SpecificationCompleted = 2,
            [Description("Development Completed")] DevelopmentCompleted = 3,
            [Description("Distribution Completed")] DistributionCompleted = 4
        }

        public override IEnumerable<FormFieldDefinition> GetFormSchema(int targetStatus)
        {
            var status = (DevelopmentStatus)targetStatus;

            return status switch
            {
                DevelopmentStatus.SpecificationCompleted => new[]
                {
                    new FormFieldDefinition { Key = "SpecificationText", Label = "Specification Details", ControlType = "textarea", IsRequired = true }
                },
                DevelopmentStatus.DevelopmentCompleted => new[]
                {
                    new FormFieldDefinition { Key = "BranchName", Label = "Git Branch Name", ControlType = "text", IsRequired = true }
                },
                DevelopmentStatus.DistributionCompleted => new[]
                {
                    new FormFieldDefinition { Key = "VersionNumber", Label = "Release Version", ControlType = "text", IsRequired = true }
                },
                _ => Array.Empty<FormFieldDefinition>()
            };
        }
    }
}
