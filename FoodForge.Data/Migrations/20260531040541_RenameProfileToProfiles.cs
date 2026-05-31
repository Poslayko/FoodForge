using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodForge.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameProfileTableToProfiles: Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dishes_profile_profile_id",
                table: "dishes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_profile",
                table: "profile");

            migrationBuilder.RenameTable(
                name: "profile",
                newName: "profiles");

            migrationBuilder.AddPrimaryKey(
                name: "pk_profiles",
                table: "profiles",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_dishes_profiles_profile_id",
                table: "dishes",
                column: "profile_id",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_dishes_profiles_profile_id",
                table: "dishes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_profiles",
                table: "profiles");

            migrationBuilder.RenameTable(
                name: "profiles",
                newName: "profile");

            migrationBuilder.AddPrimaryKey(
                name: "pk_profile",
                table: "profile",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_dishes_profile_profile_id",
                table: "dishes",
                column: "profile_id",
                principalTable: "profile",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
