using Microsoft.AspNetCore.Mvc;
using AuraDental.Aplicacion;
using AuraDental.Web.Filters;

namespace AuraDental.Web.Controllers
{
    [SessionAuthorize(RolRequerido = "Paciente")]
    public class PagosController : Controller
    {
        private readonly IPagoService _pagoService;
        private readonly IServicioService _servicioService;
        private readonly ICitaService _citaService;

        public PagosController(IPagoService pagoService, IServicioService servicioService, ICitaService citaService)
        {
            _pagoService = pagoService;
            _servicioService = servicioService;
            _citaService = citaService;
        }

        // GET: /Pagos/Iniciar?servicioId=3&bloqueAgendaId=7
        public IActionResult Iniciar(int servicioId, int bloqueAgendaId)
        {
            var servicio = _servicioService.ObtenerPorId(servicioId);
            if (servicio == null) return NotFound();

            var pacienteId = HttpContext.Session.GetInt32("UsuarioId")!.Value;

            var urlExito = Url.Action("Confirmacion", "Pagos", null, Request.Scheme)!;
            var urlCancelado = Url.Action("Consultar", "Disponibilidad", new { servicioId }, Request.Scheme)!;

            var urlCheckout = _pagoService.CrearSesionPago(
                servicioId, bloqueAgendaId, pacienteId,
                servicio.Nombre, servicio.Precio,
                urlExito, urlCancelado);

            return Redirect(urlCheckout);
        }

        // GET: /Pagos/Confirmacion?session_id=cs_test_...
        public IActionResult Confirmacion(string session_id)
        {
            var (pagado, servicioId, bloqueAgendaId, pacienteId) = _pagoService.VerificarPago(session_id);

            if (!pagado)
            {
                TempData["Mensaje"] = "El pago no se completó. Intenta agendar de nuevo.";
                TempData["Exito"] = false;
                return RedirectToAction("Index", "Disponibilidad");
            }

            // Con el pago confirmado por Stripe, ahora sí agendamos la cita
            var (exito, mensaje) = _citaService.Agendar(pacienteId, servicioId, bloqueAgendaId);

            TempData["Mensaje"] = exito
                ? "¡Pago exitoso! Tu cita ha sido agendada."
                : $"El pago se procesó, pero hubo un problema al agendar: {mensaje}. Contacta a la clínica.";
            TempData["Exito"] = exito;

            return RedirectToAction("Index", "Citas");
        }
    }
}