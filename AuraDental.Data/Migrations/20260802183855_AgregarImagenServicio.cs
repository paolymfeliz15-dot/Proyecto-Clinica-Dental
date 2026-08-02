using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDental.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Servicios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Servicios");
        }
    }
}
