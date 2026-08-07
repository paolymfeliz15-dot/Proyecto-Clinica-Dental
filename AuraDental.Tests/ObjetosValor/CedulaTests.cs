using AuraDental.Dominio.ObjetosValor;
using Xunit;

namespace AuraDental.Tests.ObjetosValor
{
    public class CedulaTests
    {
        [Theory]
        [InlineData("001-1234567-8")]
        [InlineData("00112345678")]
        public void Crear_ConFormatoValido_DebeRetornarExito(string cedula)
        {
            var (exito, _, resultado) = Cedula.Crear(cedula);

            Assert.True(exito);
            Assert.NotNull(resultado);
        }

        [Fact]
        public void Crear_SinGuiones_DebeNormalizarConGuiones()
        {
            var (_, _, cedula) = Cedula.Crear("00112345678");

            Assert.Equal("001-1234567-8", cedula!.Valor);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("abc-defghij-k")]
        [InlineData("")]
        public void Crear_ConFormatoInvalido_DebeRetornarError(string cedula)
        {
            var (exito, mensaje, resultado) = Cedula.Crear(cedula);

            Assert.False(exito);
            Assert.Null(resultado);
            Assert.NotEmpty(mensaje);
        }
    }
}