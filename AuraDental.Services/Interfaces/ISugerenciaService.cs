using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface ISugerenciaService
    {
        (bool exito, string mensaje) Crear(int pacienteId, string mensaje);
        List<Sugerencia> ObtenerTodas();
        int ContarNoLeidas();
        void MarcarComoLeida(int sugerenciaId);
    }
}
