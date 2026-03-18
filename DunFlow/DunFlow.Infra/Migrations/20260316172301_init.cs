using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DunFlow.Infra.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkTaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTaskTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WorkTaskTypeId = table.Column<int>(type: "int", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    AssignedUserId = table.Column<int>(type: "int", nullable: false),
                    CustomFieldsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodEndColumn", true),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("SqlServer:TemporalIsPeriodStartColumn", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTasks_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkTasks_WorkTaskTypes_WorkTaskTypeId",
                        column: x => x.WorkTaskTypeId,
                        principalTable: "WorkTaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WorkTasksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.CreateTable(
                name: "WorkTaskStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTaskTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusValue = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTaskStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkTaskStatuses_WorkTaskTypes_WorkTaskTypeId",
                        column: x => x.WorkTaskTypeId,
                        principalTable: "WorkTaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkTaskStatusId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ControlType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormFields_WorkTaskStatuses_WorkTaskStatusId",
                        column: x => x.WorkTaskStatusId,
                        principalTable: "WorkTaskStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName" },
                values: new object[,]
                {
                    { 1, "Nadav" },
                    { 2, "Nashef" },
                    { 3, "Dun" }
                });

            migrationBuilder.InsertData(
                table: "WorkTaskTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Software Development Task", "Development" },
                    { 2, "Purchasing and Procurement", "Procurement" }
                });

            migrationBuilder.InsertData(
                table: "WorkTaskStatuses",
                columns: new[] { "Id", "DisplayName", "IsFinal", "StatusValue", "WorkTaskTypeId" },
                values: new object[,]
                {
                    { 1, "Created", false, 1, 1 },
                    { 2, "Specification Completed", false, 2, 1 },
                    { 3, "Development Completed", false, 3, 1 },
                    { 4, "Distribution Completed", true, 4, 1 },
                    { 5, "Created", false, 1, 2 },
                    { 6, "Supplier Offers Received", false, 2, 2 },
                    { 7, "Purchasing Approval", false, 3, 2 },
                    { 8, "Receipt Image Upload", true, 4, 2 }
                });

            migrationBuilder.InsertData(
                table: "FormFields",
                columns: new[] { "Id", "ControlType", "IsRequired", "Key", "Label", "WorkTaskStatusId" },
                values: new object[,]
                {
                    { 1, "textarea", true, "SpecificationText", "Specification Details", 2 },
                    { 2, "text", true, "BranchName", "Branch Name", 3 },
                    { 3, "text", true, "VersionNumber", "Release Version", 4 },
                    { 4, "text", true, "SupplierName", "Supplier Name", 6 },
                    { 5, "number", true, "Amount", "Amount", 6 },
                    { 6, "text", true, "ApprovedBy", "Approved By", 7 },
                    { 7, "text", true, "OrderId", "Order ID", 7 },
                    { 8, "url", true, "ReceiptImageLink", "Receipt Image Link", 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormFields_WorkTaskStatusId",
                table: "FormFields",
                column: "WorkTaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_AssignedUserId",
                table: "WorkTasks",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_WorkTaskTypeId",
                table: "WorkTasks",
                column: "WorkTaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTaskStatuses_WorkTaskTypeId",
                table: "WorkTaskStatuses",
                column: "WorkTaskTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormFields");

            migrationBuilder.DropTable(
                name: "WorkTasks")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "WorkTasksHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", null)
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.DropTable(
                name: "WorkTaskStatuses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkTaskTypes");
        }
    }
}
