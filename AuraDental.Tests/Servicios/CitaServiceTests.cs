using AuraDental.Aplicacion;
using AuraDental.Dominio.Entidades;
using AuraDental.Dominio.Interfaces;
using Moq;
using Xunit;

namespace AuraDental.Tests.Servicios
{
    public class CitaServiceTests
    {
        private readonly Mock<IRepository<Cita>> _citaRepoMock;
        private readonly Mock<IRepository<Servicio>> _servicioRepoMock;
        private readonly Mock<IRepository<BloqueAgenda>> _agendaRepoMock;
        private readonly CitaService _citaService;

        public CitaServiceTests()
        {
            _citaRepoMock = new Mock<IRepository<Cita>>();
            _servicioRepoMock = new Mock<IRepository<Servicio>>();
            _agendaRepoMock = new Mock<IRepository<BloqueAgenda>>();
            _citaService = new CitaService(_citaRepoMock.Object, _servicioRepoMock.Object, _agendaRepoMock.Object);
        }

        [Fact]
        public void Agendar_ConServicioInactivo_DebeRetornarError()
        {
            var servicio = new Servicio { ServicioId = 1, Activo = false, DuracionMinutos = 30 };
            _servicioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(servicio);

            var (exito, mensaje) = _citaService.Agendar(pacienteId: 5, servicioId: 1, bloqueAgendaId: 1);

            Assert.False(exito);
            Assert.Contains("no está disponible", mensaje);
        }

        [Fact]
        public void Agendar_ConBloqueYaOcupado_DebeRetornarError()
        {
            var servicio = new Servicio { ServicioId = 1, Activo = true, DuracionMinutos = 30 };
            var bloque = new BloqueAgenda { BloqueAgendaId = 1, Disponible = false, Fecha = DateTime.Today.AddDays(1) };

            _servicioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(servicio);
            _agendaRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(bloque);

            var (exito, mensaje) = _citaService.Agendar(5, 1, 1);

            Assert.False(exito);
            Assert.Contains("ya no está disponible", mensaje);
        }

        [Fact]
        public void Agendar_ConFechaPasada_DebeRetornarError()
        {
            var servicio = new Servicio { ServicioId = 1, Activo = true, DuracionMinutos = 30 };
            var bloque = new BloqueAgenda { BloqueAgendaId = 1, Disponible = true, Fecha = DateTime.Today.AddDays(-1) };

            _servicioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(servicio);
            _agendaRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(bloque);

            var (exito, mensaje) = _citaService.Agendar(5, 1, 1);

            Assert.False(exito);
            Assert.Contains("pasada", mensaje);
        }

        [Fact]
        public void Agendar_ConDuracionInsuficiente_DebeRetornarError()
        {
            // El servicio dura 60 minutos, pero el bloque solo tiene 30
            var servicio = new Servicio { ServicioId = 1, Activo = true, DuracionMinutos = 60 };
            var bloque = new BloqueAgenda
            {
                BloqueAgendaId = 1,
                Disponible = true,
                Fecha = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(9, 30, 0)
            };

            _servicioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(servicio);
            _agendaRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(bloque);

            var (exito, mensaje) = _citaService.Agendar(5, 1, 1);

            Assert.False(exito);
            Assert.Contains("duración suficiente", mensaje);
        }

        [Fact]
        public void Agendar_ConDatosValidos_DebeCrearCitaYOcuparElBloque()
        {
            var servicio = new Servicio { ServicioId = 1, Activo = true, DuracionMinutos = 30 };
            var bloque = new BloqueAgenda
            {
                BloqueAgendaId = 1,
                Disponible = true,
                Fecha = DateTime.Today.AddDays(1),
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(10, 0, 0)
            };

            _servicioRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(servicio);
            _agendaRepoMock.Setup(r => r.ObtenerPorId(1)).Returns(bloque);

            var (exito, _) = _citaService.Agendar(pacienteId: 5, servicioId: 1, bloqueAgendaId: 1);

            Assert.True(exito);
            Assert.False(bloque.Disponible); // el bloque debe quedar ocupado tras agendar
            _citaRepoMock.Verify(r => r.Agregar(It.IsAny<Cita>()), Times.Once);
            _citaRepoMock.Verify(r => r.GuardarCambios(), Times.Once);
        }
    }
}