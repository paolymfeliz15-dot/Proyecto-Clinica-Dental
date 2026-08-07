using System.Text.RegularExpressions;

namespace AuraDental.Dominio.ObjetosValor
{
    public sealed record Cedula
    {
        // Formato dominicano: 000-0000000-0 (con o sin guiones)
        private static readonly Regex PatronConGuiones = new(@"^\d{3}-\d{7}-\d{1}$", RegexOptions.Compiled);
        private static readonly Regex PatronSinGuiones = new(@"^\d{11}$", RegexOptions.Compiled);

        public string Valor { get; }

        private Cedula(string valor)
        {
            Valor = valor;
        }

        public static (bool exito, string mensaje, Cedula? cedula) Crear(string valorCrudo)
        {
            if (string.IsNullOrWhiteSpace(valorCrudo))
                return (false, "La cédula no puede estar vacía.", null);

            var limpio = valorCrudo.Trim();

            if (!PatronConGuiones.IsMatch(limpio) && !PatronSinGuiones.IsMatch(limpio))
                return (false, "La cédula debe tener el formato 000-0000000-0 (11 dígitos).", null);

            // Normalizamos siempre al formato con guiones, sin importar cómo se escribió
            var soloDigitos = limpio.Replace("-", "");
            var normalizada = $"{soloDigitos[..3]}-{soloDigitos[3..10]}-{soloDigitos[10..]}";

            return (true, "OK", new Cedula(normalizada));
        }

        public override string ToString() => Valor;
    }
}