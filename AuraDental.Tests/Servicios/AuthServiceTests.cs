using AuraDental.Aplicacion;
using AuraDental.Aplicacion.Dtos;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Moq;
using Xunit;

namespace AuraDental.Tests.Servicios
{
    public class AuthServiceTests
    {
        private readonly Mock<IRepository<Usuario>> _usuarioRepoMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _usuarioRepoMock = new Mock<IRepository<Usuario>>();
            _emailServiceMock = new Mock<IEmailService>();
            _authService = new AuthService(_usuarioRepoMock.Object, _emailServiceMock.Object);
        }

        [Fact]
        public void CambiarPassword_ConContraseñaActualIncorrecta_DebeRetornarError()
        {
            var usuario = new Usuario
            {
                UsuarioId = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ClaveReal123")
            };
            _usuarioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(usuario);

            var (exito, mensaje) = _authService.CambiarPassword(1, "ClaveIncorrecta", "NuevaClave123");

            Assert.False(exito);
            Assert.Contains("no es correcta", mensaje);
        }

        [Fact]
        public void CambiarPassword_ConNuevaContraseñaMuyCorta_DebeRetornarError()
        {
            var usuario = new Usuario
            {
                UsuarioId = 1,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ClaveReal123")
            };
            _usuarioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(usuario);

            var (exito, mensaje) = _authService.CambiarPassword(1, "ClaveReal123", "abc");

            Assert.False(exito);
            Assert.Contains("al menos 6 caracteres", mensaje);
        }

        [Fact]
        public void CambiarPassword_ConDatosValidos_DebeActualizarElHash()
        {
            var hashOriginal = BCrypt.Net.BCrypt.HashPassword("ClaveReal123");
            var usuario = new Usuario { UsuarioId = 1, PasswordHash = hashOriginal };
            _usuarioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(usuario);

            var (exito, _) = _authService.CambiarPassword(1, "ClaveReal123", "ClaveNueva456");

            Assert.True(exito);
            Assert.NotEqual(hashOriginal, usuario.PasswordHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("ClaveNueva456", usuario.PasswordHash));
        }

        [Fact]
        public void RegistrarPaciente_ConCorreoYaRegistrado_DebeRetornarError()
        {
            var existentes = new List<Usuario> { new Usuario { Email = "paciente@correo.com" } };
            _usuarioRepoMock.Setup(r => r.Consultar()).Returns(existentes.AsQueryable());

            var datos = new RegistroPacienteDto
            {
                NombreCompleto = "Juan",
                Apellidos = "Pérez",
                Email = "paciente@correo.com",
                Cedula = "001-1234567-8",
                Password = "Clave123"
            };

            var (exito, mensaje) = _authService.RegistrarPaciente(datos);

            Assert.False(exito);
            Assert.Contains("ya está registrado", mensaje);
        }

        [Fact]
        public void RegistrarPaciente_ConEmailConFormatoInvalido_DebeRetornarError()
        {
            _usuarioRepoMock.Setup(r => r.Consultar()).Returns(new List<Usuario>().AsQueryable());

            var datos = new RegistroPacienteDto
            {
                NombreCompleto = "Juan",
                Email = "correo-sin-formato",
                Cedula = "001-1234567-8",
                Password = "Clave123"
            };

            var (exito, mensaje) = _authService.RegistrarPaciente(datos);

            Assert.False(exito);
            Assert.Contains("correo", mensaje.ToLower());
        }

        [Fact]
        public void RegistrarPaciente_ConCedulaConFormatoInvalido_DebeRetornarError()
        {
            _usuarioRepoMock.Setup(r => r.Consultar()).Returns(new List<Usuario>().AsQueryable());

            var datos = new RegistroPacienteDto
            {
                NombreCompleto = "Juan",
                Email = "juan@correo.com",
                Cedula = "123",
                Password = "Clave123"
            };

            var (exito, mensaje) = _authService.RegistrarPaciente(datos);

            Assert.False(exito);
            Assert.Contains("cédula", mensaje.ToLower());
        }

        [Fact]
        public void RegistrarPaciente_ConDatosValidos_DebeCrearUsuarioComoPaciente()
        {
            _usuarioRepoMock.Setup(r => r.Consultar()).Returns(new List<Usuario>().AsQueryable());

            var datos = new RegistroPacienteDto
            {
                NombreCompleto = "Juan",
                Apellidos = "Pérez",
                Email = "juan@correo.com",
                Cedula = "001-1234567-8",
                Password = "Clave123"
            };

            var (exito, _) = _authService.RegistrarPaciente(datos);

            Assert.True(exito);
            _usuarioRepoMock.Verify(r => r.Agregar(It.Is<Usuario>(u => u.RolId == 2 && u.Email == "juan@correo.com")), Times.Once);
            _usuarioRepoMock.Verify(r => r.GuardarCambios(), Times.Once);
        }

        [Fact]
        public void ActualizarPerfil_ConCorreoYaUsadoPorOtroUsuario_DebeRetornarError()
        {
            var usuarioAEditar = new Usuario { UsuarioId = 1, Email = "viejo@correo.com" };
            var otroUsuario = new Usuario { UsuarioId = 2, Email = "ocupado@correo.com" };

            _usuarioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(usuarioAEditar);
            _usuarioRepoMock.Setup(r => r.Consultar()).Returns(new List<Usuario> { usuarioAEditar, otroUsuario }.AsQueryable());

            var datos = new EditarPerfilDto
            {
                NombreCompleto = "Juan",
                Email = "ocupado@correo.com"
            };

            var (exito, mensaje) = _authService.ActualizarPerfil(1, datos);

            Assert.False(exito);
            Assert.Contains("otra cuenta", mensaje);
        }
    }
}