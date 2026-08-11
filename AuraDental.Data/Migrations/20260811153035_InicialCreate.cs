using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuraDental.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InicialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolId);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    ServicioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.ServicioId);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cedula = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pais = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoProvincia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FotoPerfilUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailVerificado = table.Column<bool>(type: "bit", nullable: false),
                    TokenVerificacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenExpiracion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "RolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BloquesAgenda",
                columns: table => new
                {
                    BloqueAgendaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BloquesAgenda", x => x.BloqueAgendaId);
                    table.ForeignKey(
                        name: "FK_BloquesAgenda_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resenas",
                columns: table => new
                {
                    ResenaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    Calificacion = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resenas", x => x.ResenaId);
                    table.ForeignKey(
                        name: "FK_Resenas_Usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sugerencias",
                columns: table => new
                {
                    SugerenciaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sugerencias", x => x.SugerenciaId);
                    table.ForeignKey(
                        name: "FK_Sugerencias_Usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    CitaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    ServicioId = table.Column<int>(type: "int", nullable: false),
                    BloqueAgendaId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.CitaId);
                    table.ForeignKey(
                        name: "FK_Citas_BloquesAgenda_BloqueAgendaId",
                        column: x => x.BloqueAgendaId,
                        principalTable: "BloquesAgenda",
                        principalColumn: "BloqueAgendaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Servicios_ServicioId",
                        column: x => x.ServicioId,
                        principalTable: "Servicios",
                        principalColumn: "ServicioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Expedientes",
                columns: table => new
                {
                    ExpedienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    CitaId = table.Column<int>(type: "int", nullable: true),
                    Diagnostico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tratamiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistradoPorUsuarioId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expedientes", x => x.ExpedienteId);
                    table.ForeignKey(
                        name: "FK_Expedientes_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "CitaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expedientes_Usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expedientes_Usuarios_RegistradoPorUsuarioId",
                        column: x => x.RegistradoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RolId", "Nombre" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Paciente" },
                    { 3, "Asistente" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "UsuarioId", "Activo", "Apellidos", "Cedula", "Ciudad", "Direccion", "Email", "EmailVerificado", "EstadoProvincia", "FechaCreacion", "FotoPerfilUrl", "NombreCompleto", "Pais", "PasswordHash", "RolId", "Sector", "Telefono", "TokenExpiracion", "TokenVerificacion" },
                values: new object[] { 1, true, null, null, null, null, "Admin", true, null, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Administrador", null, "$2b$11$d5.lXPkCEuYNABoynEZpQ.MR6bzSzsgCJdkHRLWlT51wrH/wiWM5W", 1, null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_BloquesAgenda_UsuarioId",
                table: "BloquesAgenda",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_BloqueAgendaId",
                table: "Citas",
                column: "BloqueAgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_PacienteId",
                table: "Citas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_ServicioId",
                table: "Citas",
                column: "ServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Expedientes_CitaId",
                table: "Expedientes",
                column: "CitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Expedientes_PacienteId",
                table: "Expedientes",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Expedientes_RegistradoPorUsuarioId",
                table: "Expedientes",
                column: "RegistradoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Resenas_PacienteId",
                table: "Resenas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_Nombre",
                table: "Servicios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sugerencias_PacienteId",
                table: "Sugerencias",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Cedula",
                table: "Usuarios",
                column: "Cedula",
                unique: true,
                filter: "[Cedula] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expedientes");

            migrationBuilder.DropTable(
                name: "Resenas");

            migrationBuilder.DropTable(
                name: "Sugerencias");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "BloquesAgenda");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
