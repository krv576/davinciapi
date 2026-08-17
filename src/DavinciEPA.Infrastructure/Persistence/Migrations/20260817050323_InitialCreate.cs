using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DavinciEPA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ResourceReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriorAuthorizationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PatientIdentifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PayerId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OrderReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Disposition = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    DispositionReason = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    TaskIdentifier = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriorAuthorizationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuleEvaluationLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorAuthorizationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EngineType = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RuleId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    InputSummary = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ResultSummary = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    EvaluatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleEvaluationLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoverageRequirementEvaluations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorAuthorizationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequirementCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    RequirementDescription = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsMet = table.Column<bool>(type: "bit", nullable: false),
                    EvaluatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverageRequirementEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoverageRequirementEvaluations_PriorAuthorizationRequests_PriorAuthorizationRequestId",
                        column: x => x.PriorAuthorizationRequestId,
                        principalTable: "PriorAuthorizationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentationRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorAuthorizationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireCanonicalUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    QuestionnaireResponseReference = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentationRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentationRequirements_PriorAuthorizationRequests_PriorAuthorizationRequestId",
                        column: x => x.PriorAuthorizationRequestId,
                        principalTable: "PriorAuthorizationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntries_Timestamp",
                table: "AuditLogEntries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageRequirementEvaluations_OrderReference",
                table: "CoverageRequirementEvaluations",
                column: "OrderReference");

            migrationBuilder.CreateIndex(
                name: "IX_CoverageRequirementEvaluations_PriorAuthorizationRequestId",
                table: "CoverageRequirementEvaluations",
                column: "PriorAuthorizationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentationRequirements_PriorAuthorizationRequestId",
                table: "DocumentationRequirements",
                column: "PriorAuthorizationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PriorAuthorizationRequests_ExternalId",
                table: "PriorAuthorizationRequests",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriorAuthorizationRequests_PatientIdentifier",
                table: "PriorAuthorizationRequests",
                column: "PatientIdentifier");

            migrationBuilder.CreateIndex(
                name: "IX_PriorAuthorizationRequests_Status",
                table: "PriorAuthorizationRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RuleEvaluationLogs_PriorAuthorizationRequestId",
                table: "RuleEvaluationLogs",
                column: "PriorAuthorizationRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogEntries");

            migrationBuilder.DropTable(
                name: "CoverageRequirementEvaluations");

            migrationBuilder.DropTable(
                name: "DocumentationRequirements");

            migrationBuilder.DropTable(
                name: "RuleEvaluationLogs");

            migrationBuilder.DropTable(
                name: "PriorAuthorizationRequests");
        }
    }
}
