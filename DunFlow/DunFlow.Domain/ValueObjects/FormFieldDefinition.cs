using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DunFlow.Domain.ValueObjects
{
    public class FormFieldDefinition
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string ControlType { get; set; }
        public bool IsRequired { get; set; }
    }
}
