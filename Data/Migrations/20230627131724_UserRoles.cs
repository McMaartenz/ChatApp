using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Data.Migrations
{
    public partial class UserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6557c6c9-85c9-48ab-9f80-22d9011c4985", "8222e8b9-9744-44f2-a61d-23f1308a0022", "ChatModerator", "CHATMODERATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b3c7c71d-44ac-4f8c-8b65-9c9513d9affc", "f652e5a4-7f6f-4033-b036-a7808766c5cd", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6557c6c9-85c9-48ab-9f80-22d9011c4985");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b3c7c71d-44ac-4f8c-8b65-9c9513d9affc");
        }
    }
}
