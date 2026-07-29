using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDental.Data.Migrations
{
    /// <inheritdoc />
    public partial class AmpliarUsuarioParaPacientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellidos",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pais",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinciaId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 1,
                columns: new[] { "Apellidos", "Cedula", "Direccion", "Pais", "ProvinciaId", "Sector", "Telefono" },
                values: new object[] { null, null, null, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cedula",
                table: "Usuarios",
                column: "Cedula",
                unique: true,
                filter: "[Cedula] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ProvinciaId",
                table: "Usuarios",
                column: "ProvinciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Provincias_ProvinciaId",
                table: "Usuarios",
                column: "ProvinciaId",
                principalTable: "Provincias",
                principalColumn: "ProvinciaId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Provincias_ProvinciaId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Cedula",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ProvinciaId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Apellidos",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Pais",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Usuarios");
        }
    }
}
