using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Moq;
using Xunit;

namespace AuraDental.Tests.Servicios
{
    public class ServicioServiceTests
    {
        private readonly Mock<IRepository<Servicio>> _repoMock;
        private readonly ServicioService _servicioService;

        public ServicioServiceTests()
        {
            _repoMock = new Mock<IRepository<Servicio>>();
            _servicioService = new ServicioService(_repoMock.Object);
        }

        [Fact]
        public void Crear_ConPrecioNegativo_DebeRetornarError()
        {
            var servicio = new Servicio { Nombre = "Limpieza", Precio = -50 };

            var (exito, mensaje) = _servicioService.Crear(servicio);

            Assert.False(exito);
            Assert.Contains("negativo", mensaje);
        }

        [Fact]
        public void Crear_ConPrecioValido_DebeQuedarActivo()
        {
            var servicio = new Servicio { Nombre = "Limpieza", Precio = 1500 };

            var (exito, _) = _servicioService.Crear(servicio);

            Assert.True(exito);
            Assert.True(servicio.Activo);
            _repoMock.Verify(r => r.GuardarCambios(), Times.Once);
        }

        [Fact]
        public void ExisteNombre_ConNombreDuplicado_DebeRetornarVerdadero()
        {
            var existentes = new List<Servicio> { new Servicio { ServicioId = 1, Nombre = "Limpieza" } };
            _repoMock.Setup(r => r.Consultar()).Returns(existentes.AsQueryable());

            var resultado = _servicioService.ExisteNombre("Limpieza");

            Assert.True(resultado);
        }

        [Fact]
        public void ExisteNombre_ExcluyendoElMismoRegistro_DebeRetornarFalso()
        {
            var existentes = new List<Servicio> { new Servicio { ServicioId = 1, Nombre = "Limpieza" } };
            _repoMock.Setup(r => r.Consultar()).Returns(existentes.AsQueryable());

            // Editando el servicio 1 sin cambiar su propio nombre no debe marcarse como duplicado
            var resultado = _servicioService.ExisteNombre("Limpieza", idExcluir: 1);

            Assert.False(resultado);
        }
    }
}