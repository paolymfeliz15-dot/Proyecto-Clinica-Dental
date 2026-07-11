using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Data.Entities;

namespace AuraDental.Services
{
    public interface IAuthService
    {
        Usuario? ValidarCredenciales(string email, string password);
        Usuario RegistrarUsuario(string nombreCompleto, string email, string password, int rolId);
        bool ExisteEmail(string email);
    }
}