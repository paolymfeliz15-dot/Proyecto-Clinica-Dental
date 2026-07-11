using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuraDental.Web.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        // Si se especifica, solo ese rol puede pasar. Si se deja vacío,
        // solo exige que haya una sesión activa (cualquier rol).
        public string? RolRequerido { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuarioId = context.HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Cuenta", null);
                return;
            }

            if (!string.IsNullOrEmpty(RolRequerido))
            {
                var rolActual = context.HttpContext.Session.GetString("Rol");

                if (rolActual != RolRequerido)
                {
                    // Usuario autenticado pero con rol equivocado para esta sección
                    context.Result = new RedirectToActionResult("AccesoDenegado", "Cuenta", null);
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}