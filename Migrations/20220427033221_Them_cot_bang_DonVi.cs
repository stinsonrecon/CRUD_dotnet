using Microsoft.EntityFrameworkCore.Migrations;

namespace CRUD.Migrations
{
    public partial class Them_cot_bang_DonVi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LaPhongBan",
                table: "DonVi",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaPhongBan",
                table: "DonVi");
        }
    }
}
