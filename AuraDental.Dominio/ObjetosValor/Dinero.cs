namespace AuraDental.Dominio.ObjetosValor
{
    public sealed record Dinero
    {
        public decimal Monto { get; }

        private Dinero(decimal monto)
        {
            Monto = monto;
        }

        public static (bool exito, string mensaje, Dinero? dinero) Crear(decimal montoCrudo)
        {
            if (montoCrudo < 0)
                return (false, "El monto no puede ser negativo.", null);

            if (montoCrudo > 1_000_000)
                return (false, "El monto excede el límite permitido.", null);

            // Redondeamos siempre a 2 decimales, como cualquier valor monetario real
            var normalizado = Math.Round(montoCrudo, 2);

            return (true, "OK", new Dinero(normalizado));
        }

        public static Dinero Sumar(Dinero a, Dinero b) => new(a.Monto + b.Monto);

        public override string ToString() => Monto.ToString("C");
    }
}