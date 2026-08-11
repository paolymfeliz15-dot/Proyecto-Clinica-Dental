using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AuraDental.Aplicacion
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public EmailService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> EnviarCorreoVerificacionAsync(string destinatarioEmail, string nombreDestinatario, string token)
        {
            var apiKey = _configuration["Brevo:ApiKey"];
            var remitenteEmail = _configuration["Brevo:RemitenteEmail"];
            var remitenteNombre = _configuration["Brevo:RemitenteNombre"];

            var enlaceVerificacion = $"https://localhost:7265/Cuenta/VerificarCorreo?token={token}";

            var payload = new
            {
                sender = new { name = remitenteNombre, email = remitenteEmail },
                to = new[] { new { email = destinatarioEmail, name = nombreDestinatario } },
                subject = "Verifica tu correo — AuraDental",
                htmlContent = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 500px;'>
                        <h2 style='color: #0B2545;'>¡Hola, {nombreDestinatario}!</h2>
                        <p>Gracias por registrarte en AuraDental. Confirma tu correo haciendo clic en el siguiente botón:</p>
                        <a href='{enlaceVerificacion}' style='background:#0B2545;color:white;padding:12px 24px;text-decoration:none;border-radius:8px;display:inline-block;'>
                            Verificar mi correo
                        </a>
                        <p style='color:#667085;font-size:13px;margin-top:20px;'>Este enlace expira en 24 horas.</p>
                    </div>"
            };

            var contenido = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email")
            {
                Content = contenido
            };
            request.Headers.Add("api-key", apiKey);
            request.Headers.Add("accept", "application/json");

            try
            {
                var respuesta = await _httpClient.SendAsync(request);
                return respuesta.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }
    }
}