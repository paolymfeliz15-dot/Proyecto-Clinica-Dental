using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Moq;
using Xunit;

namespace AuraDental.Tests.Servicios
{
    public class AgendaServiceTests
    {
        private readonly Mock<IRepository<BloqueAgenda>> _agendaRepoMock;
        private readonly Mock<IRepository<Servicio>> _servicioRepoMock;
        private readonly AgendaService _agendaService;

        public AgendaServiceTests()
        {
            _agendaRepoMock = new Mock<IRepository<BloqueAgenda>>();
            _servicioRepoMock = new Mock<IRepository<Servicio>>();
            _agendaService = new AgendaService(_agendaRepoMock.Object, _servicioRepoMock.Object);
        }

        [Fact]
        public void ExisteSolapamiento_ConHorarioQueNoChoca_DebeRetornarFalso()
        {
            // Arrange: un bloque existente de 9:00 a 10:00
            var bloques = new List<BloqueAgenda>
            {
                new BloqueAgenda
                {
                    BloqueAgendaId = 1,
                    Fecha = new DateTime(2026, 8, 10),
                    HoraInicio = new TimeSpan(9, 0, 0),
                    HoraFin = new TimeSpan(10, 0, 0)
                }
            };
            _agendaRepoMock.Setup(r => r.Consultar()).Returns(bloques.AsQueryable());

            // Act: consultamos un nuevo horario de 10:00 a 11:00 (justo después, sin cruce)
            var resultado = _agendaService.ExisteSolapamiento(
                new DateTime(2026, 8, 10), new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0));

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public void ExisteSolapamiento_ConHorarioQueChocaAMitad_DebeRetornarVerdadero()
        {
            var bloques = new List<BloqueAgenda>
            {
                new BloqueAgenda
                {
                    BloqueAgendaId = 1,
                    Fecha = new DateTime(2026, 8, 10),
                    HoraInicio = new TimeSpan(9, 0, 0),
                    HoraFin = new TimeSpan(10, 0, 0)
                }
            };
            _agendaRepoMock.Setup(r => r.Consultar()).Returns(bloques.AsQueryable());

            // Nuevo horario de 9:30 a 10:30 -> se cruza con el existente
            var resultado = _agendaService.ExisteSolapamiento(
                new DateTime(2026, 8, 10), new TimeSpan(9, 30, 0), new TimeSpan(10, 30, 0));

            Assert.True(resultado);
        }

        [Fact]
        public void ExisteSolapamiento_EnFechaDistinta_DebeRetornarFalso()
        {
            var bloques = new List<BloqueAgenda>
            {
                new BloqueAgenda
                {
                    BloqueAgendaId = 1,
                    Fecha = new DateTime(2026, 8, 10),
                    HoraInicio = new TimeSpan(9, 0, 0),
                    HoraFin = new TimeSpan(10, 0, 0)
                }
            };
            _agendaRepoMock.Setup(r => r.Consultar()).Returns(bloques.AsQueryable());

            // Mismo horario, pero un día distinto -> no debe chocar
            var resultado = _agendaService.ExisteSolapamiento(
                new DateTime(2026, 8, 11), new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0));

            Assert.False(resultado);
        }

        [Fact]
        public void Crear_ConHoraInicioMayorQueHoraFin_DebeRetornarError()
        {
            _agendaRepoMock.Setup(r => r.Consultar()).Returns(new List<BloqueAgenda>().AsQueryable());

            var bloque = new BloqueAgenda
            {
                Fecha = new DateTime(2026, 8, 10),
                HoraInicio = new TimeSpan(11, 0, 0),
                HoraFin = new TimeSpan(9, 0, 0) // fin antes que inicio -> inválido
            };

            var (exito, mensaje) = _agendaService.Crear(bloque);

            Assert.False(exito);
            Assert.Contains("anterior", mensaje);
        }

        [Fact]
        public void Crear_ConHorarioValidoYSinSolapamiento_DebeRetornarExito()
        {
            _agendaRepoMock.Setup(r => r.Consultar()).Returns(new List<BloqueAgenda>().AsQueryable());

            var bloque = new BloqueAgenda
            {
                Fecha = new DateTime(2026, 8, 10),
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(10, 0, 0)
            };

            var (exito, _) = _agendaService.Crear(bloque);

            Assert.True(exito);
            _agendaRepoMock.Verify(r => r.Agregar(bloque), Times.Once);
            _agendaRepoMock.Verify(r => r.GuardarCambios(), Times.Once);
        }
    }
}