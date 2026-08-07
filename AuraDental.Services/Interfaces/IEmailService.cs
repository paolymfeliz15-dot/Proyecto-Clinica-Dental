using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDental.Aplicacion
{
    public interface IEmailService
    {
        Task<bool> EnviarCorreoVerificacionAsync(string destinatarioEmail, string nombreDestinatario, string token);
    }
}