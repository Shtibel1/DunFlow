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
    public class ProcurementStrategy : BaseTaskStrategy<ProcurementStrategy.ProcurementStatus>
    {
        public override TaskType TaskType => TaskType.Procurement;
        public override int FinalStatus => 4;

        public enum ProcurementStatus
        {
            [Description("Created")]
            Created = 1,

            [Description("Supplier Offers Received")]
            SupplierOffersReceived = 2,

            [Description("Purchasing Approval")]
            PurchasingApproval = 3,

            [Description("Receipt Image Upload")]
            ReceiptImageUpload = 4
        }

        public override IEnumerable<FormFieldDefinition> GetFormSchema(int targetStatus)
        {
            var status = (ProcurementStatus)targetStatus;

            return status switch
            {
                ProcurementStatus.SupplierOffersReceived => new[]
                {
                    new FormFieldDefinition { Key = "SupplierName", Label = "Supplier Name", ControlType = "text", IsRequired = true },
                    new FormFieldDefinition { Key = "Amount", Label = "Amount", ControlType = "number", IsRequired = true }
                },
                ProcurementStatus.PurchasingApproval => new[]
                {
                    new FormFieldDefinition { Key = "ApprovedBy", Label = "Approved By", ControlType = "text", IsRequired = true },
                    new FormFieldDefinition { Key = "OrderId", Label = "Order ID", ControlType = "text", IsRequired = true }
                },
                ProcurementStatus.ReceiptImageUpload => new[]
                {
                    new FormFieldDefinition { Key = "ReceiptImageLink", Label = "Receipt Image Link", ControlType = "url", IsRequired = true }
                },
                _ => Array.Empty<FormFieldDefinition>()
            };
        }
    }
    
}
