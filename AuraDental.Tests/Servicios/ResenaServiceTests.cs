using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Moq;
using Xunit;

namespace AuraDental.Tests.Servicios
{
    public class ResenaServiceTests
    {
        private readonly Mock<IRepository<Resena>> _repoMock;
        private readonly ResenaService _resenaService;

        public ResenaServiceTests()
        {
            _repoMock = new Mock<IRepository<Resena>>();
            _resenaService = new ResenaService(_repoMock.Object);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        public void Crear_ConCalificacionFueraDeRango_DebeRetornarError(int calificacion)
        {
            var (exito, mensaje) = _resenaService.Crear(1, calificacion, "Comentario válido");

            Assert.False(exito);
            Assert.Contains("entre 1 y 5", mensaje);
        }

        [Fact]
        public void Crear_ConComentarioVacio_DebeRetornarError()
        {
            var (exito, mensaje) = _resenaService.Crear(1, 5, "");

            Assert.False(exito);
            Assert.Contains("vacío", mensaje);
        }

        [Fact]
        public void Crear_ConDatosValidos_DebeGuardarLaResena()
        {
            var (exito, _) = _resenaService.Crear(1, 5, "Muy buen servicio");

            Assert.True(exito);
            _repoMock.Verify(r => r.Agregar(It.Is<Resena>(r => r.Calificacion == 5)), Times.Once);
            _repoMock.Verify(r => r.GuardarCambios(), Times.Once);
        }
    }
}