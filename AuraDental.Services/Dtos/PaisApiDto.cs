using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AuraDental.Services.Dtos
{
    public class PaisesRespuestaDto
    {
        [JsonPropertyName("data")]
        public List<PaisDto> Data { get; set; } = new();
    }

    public class PaisDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class EstadosRespuestaDto
    {
        [JsonPropertyName("data")]
        public EstadosDataDto? Data { get; set; }
    }

    public class EstadosDataDto
    {
        [JsonPropertyName("states")]
        public List<EstadoDto> States { get; set; } = new();
    }

    public class EstadoDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class CiudadesRespuestaDto
    {
        [JsonPropertyName("data")]
        public List<string> Data { get; set; } = new();
    }
}