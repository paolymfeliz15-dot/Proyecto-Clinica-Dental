using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AuraDental.Services
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
            var apiToken = _configuration["Mailtrap:ApiToken"];
            var inboxId = _configuration["Mailtrap:InboxId"];
            var remitenteEmail = _configuration["Mailtrap:RemitenteEmail"];
            var remitenteNombre = _configuration["Mailtrap:RemitenteNombre"];

            var enlaceVerificacion = $"https://localhost:7265/Cuenta/VerificarCorreo?token={token}";

            var payload = new
            {
                from = new { email = remitenteEmail, name = remitenteNombre },
                to = new[] { new { email = destinatarioEmail } },
                subject = "Verifica tu correo — AuraDental",
                html = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 500px;'>
                        <h2 style='color: #14786C;'>¡Hola, {nombreDestinatario}!</h2>
                        <p>Gracias por registrarte en AuraDental. Confirma tu correo haciendo clic en el siguiente botón:</p>
                        <a href='{enlaceVerificacion}' style='background:#14786C;color:white;padding:12px 24px;text-decoration:none;border-radius:8px;display:inline-block;'>
                            Verificar mi correo
                        </a>
                        <p style='color:#75868A;font-size:13px;margin-top:20px;'>Este enlace expira en 24 horas.</p>
                    </div>"
            };

            var contenido = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://sandbox.api.mailtrap.io/api/send/{inboxId}")
            {
                Content = contenido
            };
            request.Headers.Add("Authorization", $"Bearer {apiToken}");

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
