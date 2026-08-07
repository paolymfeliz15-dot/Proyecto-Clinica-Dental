using AuraDental.Dominio.ObjetosValor;
using Xunit;

namespace AuraDental.Tests.ObjetosValor
{
    public class DineroTests
    {
        [Fact]
        public void Crear_ConMontoValido_DebeRetornarExito()
        {
            var (exito, _, dinero) = Dinero.Crear(1500.50m);

            Assert.True(exito);
            Assert.Equal(1500.50m, dinero!.Monto);
        }

        [Fact]
        public void Crear_ConMontoNegativo_DebeRetornarError()
        {
            var (exito, mensaje, dinero) = Dinero.Crear(-100m);

            Assert.False(exito);
            Assert.Null(dinero);
        }

        [Fact]
        public void Crear_DebeRedondearADosDecimales()
        {
            var (_, _, dinero) = Dinero.Crear(99.9999m);

            Assert.Equal(100.00m, dinero!.Monto);
        }

        [Fact]
        public void Sumar_DebeCombinarDosMontos()
        {
            var (_, _, a) = Dinero.Crear(100m);
            var (_, _, b) = Dinero.Crear(50m);

            var resultado = Dinero.Sumar(a!, b!);

            Assert.Equal(150m, resultado.Monto);
        }
    }
}