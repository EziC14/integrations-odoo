using Microsoft.Extensions.Configuration;
using OdooCls.Application.Dtos;
using OdooCls.Application.Mapper;
using OdooCls.Core.Entities;
using OdooCls.Core.Interfaces;

namespace OdooCls.Application.Services
{
    public class RegistroArticulosServices
    {
        private readonly IConfiguration configuration;
        private readonly IRegistroArticulosRepository repo;

        public RegistroArticulosServices(IConfiguration configuration, IRegistroArticulosRepository repo)
        {
            this.configuration = configuration;
            this.repo = repo;
        }

        public async Task<ApiResponse<RegistroArticulosDto>> CreateAsync(RegistroArticulosDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroArticulosDto>(400, 1, "No se recibio datos en el Archivo");

                // Duplicado
                if (await repo.ExisteArticulo(dto.ARTCOD))
                    return new ApiResponse<RegistroArticulosDto>(400, 2007, $"Artículo {dto.ARTCOD} ya existe");

                var entity = RegistroArticulosMapper.DtoToEntity(dto);
                var ok = await repo.InsertTarti(entity);
                if (ok)
                    return new ApiResponse<RegistroArticulosDto>(200, 1000, "Artículo registrado correctamente");

                return new ApiResponse<RegistroArticulosDto>(400, 1001, "No se pudo registrar el artículo");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroArticulosDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegistroArticulosDto>> UpdateAsync(RegistroArticulosDto dto)
        {
            try
            {
                if (dto == null)
                    return new ApiResponse<RegistroArticulosDto>(400, 1, "No se recibio datos en el Archivo");

                if (string.IsNullOrWhiteSpace(dto.ARTCOD))
                    return new ApiResponse<RegistroArticulosDto>(400, 2009, "ARTCOD es obligatorio");

                if (!await repo.ExisteArticulo(dto.ARTCOD))
                    return new ApiResponse<RegistroArticulosDto>(404, 2008, $"Artículo {dto.ARTCOD} no existe");

                if (string.IsNullOrWhiteSpace(dto.ARTDES))
                    return new ApiResponse<RegistroArticulosDto>(400, 2011, "ARTDES (Nombre del artículo) es obligatorio para actualizar");

                if (string.IsNullOrWhiteSpace(dto.ARSITU))
                    return new ApiResponse<RegistroArticulosDto>(400, 2012, "ARSITU (Situación) es obligatorio para actualizar");

                var ok = await repo.UpdateDescripcionYSituacion(dto.ARTCOD, dto.ARTDES, dto.ARSITU);
                if (ok)
                    return new ApiResponse<RegistroArticulosDto>(200, 1000, "Artículo actualizado correctamente");

                return new ApiResponse<RegistroArticulosDto>(400, 1001, "No se pudo actualizar el artículo");
            }
            catch (Exception ex)
            {
                return new ApiResponse<RegistroArticulosDto>(500, 500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
