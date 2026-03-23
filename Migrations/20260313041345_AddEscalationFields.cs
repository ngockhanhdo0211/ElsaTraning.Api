using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaTraning.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEscalationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EscalatedAt",
                table: "ApprovalRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EscalationReason",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalatedAt",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "EscalationReason",
                table: "ApprovalRequests");
        }
    }
}
