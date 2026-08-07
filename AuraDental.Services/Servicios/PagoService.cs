using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace AuraDental.Aplicacion
{
    public class PagoService : IPagoService
    {
        public PagoService(IConfiguration configuration)
        {
            Stripe.StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
        }

        public string CrearSesionPago(int servicioId, int bloqueAgendaId, int pacienteId, string nombreServicio, decimal precio, string urlExito, string urlCancelado)
        {
            var opciones = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmountDecimal = precio * 100, // Stripe trabaja en centavos
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"AuraDental — {nombreServicio}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = urlExito + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = urlCancelado,
                // Guardamos los datos necesarios para agendar la cita DESPUÉS de confirmar el pago
                Metadata = new Dictionary<string, string>
                {
                    { "servicioId", servicioId.ToString() },
                    { "bloqueAgendaId", bloqueAgendaId.ToString() },
                    { "pacienteId", pacienteId.ToString() }
                }
            };

            var servicio = new SessionService();
            Session sesion = servicio.Create(opciones);

            return sesion.Url;
        }

        public (bool pagado, int servicioId, int bloqueAgendaId, int pacienteId) VerificarPago(string sessionId)
        {
            var servicio = new SessionService();
            Session sesion = servicio.Get(sessionId);

            bool pagado = sesion.PaymentStatus == "paid";

            int servicioId = int.Parse(sesion.Metadata["servicioId"]);
            int bloqueAgendaId = int.Parse(sesion.Metadata["bloqueAgendaId"]);
            int pacienteId = int.Parse(sesion.Metadata["pacienteId"]);

            return (pagado, servicioId, bloqueAgendaId, pacienteId);
        }
    }
}