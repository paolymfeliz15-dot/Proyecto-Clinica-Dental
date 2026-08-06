namespace AuraDental.Aplicacion
{
    public interface IPagoService
    {
        string CrearSesionPago(int servicioId, int bloqueAgendaId, int pacienteId, string nombreServicio, decimal precio, string urlExito, string urlCancelado);
        (bool pagado, int servicioId, int bloqueAgendaId, int pacienteId) VerificarPago(string sessionId);
    }
}