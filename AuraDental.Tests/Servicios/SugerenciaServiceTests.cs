using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Moq;
using Xunit;

namespace AuraDental.Tests.Servicios
{
    public class SugerenciaServiceTests
    {
        private readonly Mock<IRepository<Sugerencia>> _repoMock;
        private readonly SugerenciaService _sugerenciaService;

        public SugerenciaServiceTests()
        {
            _repoMock = new Mock<IRepository<Sugerencia>>();
            _sugerenciaService = new SugerenciaService(_repoMock.Object);
        }

        [Fact]
        public void Crear_ConMensajeVacio_DebeRetornarError()
        {
            var (exito, mensaje) = _sugerenciaService.Crear(1, "   ");

            Assert.False(exito);
            Assert.Contains("vacío", mensaje);
        }

        [Fact]
        public void Crear_ConMensajeValido_DebeQuedarComoNoLeida()
        {
            var (exito, _) = _sugerenciaService.Crear(1, "Excelente atención");

            Assert.True(exito);
            _repoMock.Verify(r => r.Agregar(It.Is<Sugerencia>(s => !s.Leida && s.PacienteId == 1)), Times.Once);
        }

        [Fact]
        public void ContarNoLeidas_DebeContarSoloLasNoLeidas()
        {
            var sugerencias = new List<Sugerencia>
            {
                new Sugerencia { Leida = false },
                new Sugerencia { Leida = true },
                new Sugerencia { Leida = false }
            };
            _repoMock.Setup(r => r.Consultar()).Returns(sugerencias.AsQueryable());

            var total = _sugerenciaService.ContarNoLeidas();

            Assert.Equal(2, total);
        }

        [Fact]
        public void MarcarComoLeida_DebeActualizarElCampoLeida()
        {
            var sugerencia = new Sugerencia { SugerenciaId = 1, Leida = false };
            _repoMock.Setup(r => r.ObtenerPorId(1)).Returns(sugerencia);

            _sugerenciaService.MarcarComoLeida(1);

            Assert.True(sugerencia.Leida);
            _repoMock.Verify(r => r.GuardarCambios(), Times.Once);
        }
    }
}