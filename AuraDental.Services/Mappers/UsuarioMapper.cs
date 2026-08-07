using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuraDental.Aplicacion.Dtos;
using AuraDental.Dominio.Entidades;

namespace AuraDental.Aplicacion.Mappers
{
    public static class UsuarioMapper
    {
        // ===== DTO de entrada -> Entidad =====

        public static Usuario ARegistroPaciente(RegistroPacienteDto dto)
        {
            return new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                Apellidos = dto.Apellidos,
                Cedula = dto.Cedula,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                Pais = dto.Pais,
                EstadoProvincia = dto.EstadoProvincia,
                Ciudad = dto.Ciudad,
                Sector = dto.Sector
                // RolId, Activo, PasswordHash, etc. los asigna el servicio, no el mapeador
            };
        }

        public static Usuario APersonalNuevo(PersonalDto dto)
        {
            return new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                Apellidos = dto.Apellidos,
                Cedula = dto.Cedula,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                Pais = dto.Pais,
                EstadoProvincia = dto.EstadoProvincia,
                Ciudad = dto.Ciudad,
                Sector = dto.Sector,
                RolId = dto.RolId
            };
        }

        public static void ActualizarDesdePersonalDto(Usuario usuario, PersonalDto dto)
        {
            usuario.NombreCompleto = dto.NombreCompleto;
            usuario.Apellidos = dto.Apellidos;
            usuario.Cedula = dto.Cedula;
            usuario.Telefono = dto.Telefono;
            usuario.Email = dto.Email;
            usuario.Direccion = dto.Direccion;
            usuario.Pais = dto.Pais;
            usuario.EstadoProvincia = dto.EstadoProvincia;
            usuario.Ciudad = dto.Ciudad;
            usuario.Sector = dto.Sector;
            usuario.RolId = dto.RolId;
            // PasswordHash NUNCA se toca aquí, igual que antes
        }

        // ===== Entidad -> DTO de salida =====

        public static UsuarioResumenDto AResumen(Usuario usuario)
        {
            return new UsuarioResumenDto
            {
                UsuarioId = usuario.UsuarioId,
                NombreCompleto = usuario.NombreCompleto,
                Apellidos = usuario.Apellidos ?? string.Empty,
                Email = usuario.Email,
                Telefono = usuario.Telefono ?? string.Empty,
                Cedula = usuario.Cedula ?? string.Empty,
                Direccion = usuario.Direccion ?? string.Empty,
                Pais = usuario.Pais ?? string.Empty,
                EstadoProvincia = usuario.EstadoProvincia ?? string.Empty,
                Ciudad = usuario.Ciudad ?? string.Empty,
                Sector = usuario.Sector ?? string.Empty,
                NombreRol = usuario.Rol?.Nombre ?? string.Empty,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion
            };
        }

        public static List<UsuarioResumenDto> AResumenLista(List<Usuario> usuarios)
        {
            return usuarios.Select(AResumen).ToList();
        }
    }
}