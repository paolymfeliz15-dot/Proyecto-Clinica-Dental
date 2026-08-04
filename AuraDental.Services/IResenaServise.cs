using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion
{
    public interface IResenaService
    {
        List<Resena> ObtenerTodas();
        (bool exito, string mensaje) Crear(int pacienteId, int calificacion, string comentario);
    }
}