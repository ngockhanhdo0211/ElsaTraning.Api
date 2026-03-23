using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ElsaTraning.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEscalationFieldsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EscalationNote",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EscalationNote",
                table: "ApprovalRequests");
        }
    }
}
