using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Moq;
using Xunit;

namespace AuraDental.Tests.Servicios
{
    public class ExpedienteServiceTests
    {
        private readonly Mock<IRepository<Expediente>> _expedienteRepoMock;
        private readonly Mock<IRepository<Cita>> _citaRepoMock;
        private readonly ExpedienteService _expedienteService;

        public ExpedienteServiceTests()
        {
            _expedienteRepoMock = new Mock<IRepository<Expediente>>();
            _citaRepoMock = new Mock<IRepository<Cita>>();
            _expedienteService = new ExpedienteService(_expedienteRepoMock.Object, _citaRepoMock.Object);
        }

        [Fact]
        public void Crear_ConDiagnosticoVacio_DebeRetornarError()
        {
            var (exito, mensaje) = _expedienteService.Crear(1, 2, "", "Tratamiento", null);

            Assert.False(exito);
            Assert.Contains("obligatorios", mensaje);
        }

        [Fact]
        public void Crear_ConCitaInexistente_DebeRetornarError()
        {
            _citaRepoMock.Setup(r => r.ObtenerPorId(1)).Returns((Cita?)null);

            var (exito, mensaje) = _expedienteService.Crear(1, 2, "Diagnóstico", "Tratamiento", null);

            Assert.False(exito);
            Assert.Contains("no existe", mensaje);
        }

        [Fact]
        public void Crear_ConCitaYaCompletada_DebeRetornarError()
        {
            var cita = new Cita { CitaId = 1, Estado = EstadoCita.Completada };
            _citaRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(cita);

            var (exito, mensaje) = _expedienteService.Crear(1, 2, "Diagnóstico", "Tratamiento", null);

            Assert.False(exito);
            Assert.Contains("Agendada", mensaje);
        }

        [Fact]
        public void Crear_ConDatosValidos_DebeMarcarLaCitaComoCompletada()
        {
            var cita = new Cita { CitaId = 1, PacienteId = 5, Estado = EstadoCita.Agendada };
            _citaRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(cita);

            var (exito, _) = _expedienteService.Crear(1, registradoPorUsuarioId: 2,
                diagnostico: "Caries", tratamiento: "Resina", observaciones: null);

            Assert.True(exito);
            Assert.Equal(EstadoCita.Completada, cita.Estado);
            _expedienteRepoMock.Verify(r => r.Agregar(It.IsAny<Expediente>()), Times.Once);
        }
    }
}