using System.Text.RegularExpressions;

namespace AuraDental.Dominio.ObjetosValor
{
    // Un Objeto de Valor: inmutable, se valida a sí mismo al crearse,
    // y dos instancias con el mismo Valor son iguales entre sí (gracias a 'record').
    public sealed record Email
    {
        private static readonly Regex PatronValido = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public string Valor { get; }

        private Email(string valor)
        {
            Valor = valor;
        }

        public static (bool exito, string mensaje, Email? email) Crear(string valorCrudo)
        {
            if (string.IsNullOrWhiteSpace(valorCrudo))
                return (false, "El correo no puede estar vacío.", null);

            var normalizado = valorCrudo.Trim().ToLowerInvariant();

            if (!PatronValido.IsMatch(normalizado))
                return (false, "El formato del correo no es válido.", null);

            return (true, "OK", new Email(normalizado));
        }

        public override string ToString() => Valor;
    }
}