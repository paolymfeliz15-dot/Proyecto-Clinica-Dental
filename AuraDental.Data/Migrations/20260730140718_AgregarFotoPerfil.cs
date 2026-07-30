using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDental.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFotoPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilUrl",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 1,
                column: "FotoPerfilUrl",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPerfilUrl",
                table: "Usuarios");
        }
    }
}
