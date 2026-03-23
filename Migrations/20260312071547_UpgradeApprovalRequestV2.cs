using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaTraning.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeApprovalRequestV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentApprover",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentStep",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "ApprovalRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "ApprovalRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentApprover",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "CurrentStep",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "RejectedReason",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "ApprovalRequests");
        }
    }
}
