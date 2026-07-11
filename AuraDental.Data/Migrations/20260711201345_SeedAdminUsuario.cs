using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuraDental.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "UsuarioId", "Activo", "Email", "FechaCreacion", "NombreCompleto", "PasswordHash", "RolId" },
                values: new object[] { 1, true, "Admin", new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador", "$2b$11$d5.lXPkCEuYNABoynEZpQ.MR6bzSzsgCJdkHRLWlT51wrH/wiWM5W", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "UsuarioId",
                keyValue: 1);
        }
    }
}
