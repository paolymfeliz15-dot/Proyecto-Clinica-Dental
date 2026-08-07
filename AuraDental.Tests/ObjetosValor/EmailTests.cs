using AuraDental.Dominio.ObjetosValor;
using Xunit;

namespace AuraDental.Tests.ObjetosValor
{
    public class EmailTests
    {
        [Theory]
        [InlineData("paciente@correo.com")]
        [InlineData("Admin.Uno@auradental.com")]
        public void Crear_ConFormatoValido_DebeRetornarExito(string correo)
        {
            var (exito, _, email) = Email.Crear(correo);

            Assert.True(exito);
            Assert.NotNull(email);
        }

        [Theory]
        [InlineData("")]
        [InlineData("sin-arroba.com")]
        [InlineData("sin-dominio@")]
        [InlineData("   ")]
        public void Crear_ConFormatoInvalido_DebeRetornarError(string correo)
        {
            var (exito, mensaje, email) = Email.Crear(correo);

            Assert.False(exito);
            Assert.Null(email);
            Assert.NotEmpty(mensaje);
        }

        [Fact]
        public void Crear_DebeNormalizarAMinusculas()
        {
            var (_, _, email) = Email.Crear("Paciente@Correo.COM");

            Assert.Equal("paciente@correo.com", email!.Valor);
        }

        [Fact]
        public void DosEmails_ConElMismoValor_DebenSerIguales()
        {
            var (_, _, email1) = Email.Crear("test@correo.com");
            var (_, _, email2) = Email.Crear("test@correo.com");

            // Esto es lo que caracteriza a un Objeto de Valor: igualdad por VALOR, no por referencia
            Assert.Equal(email1, email2);
        }
    }
}