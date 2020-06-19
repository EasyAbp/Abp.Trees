using System;
using System.Threading.Tasks;
using EasyAbp.Abp.Trees.Dtos;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Abp.Trees
{
    public interface ITreeAppService<TGetOutputDto, TGetListInput, TCreateInput, TUpdateInput, TMoveInput>
        where TGetOutputDto : ITreeDto
        where TCreateInput : ICreateInput
        where TUpdateInput : IUpdateInput
        where TMoveInput : IMoveInput
    {
        Task<TGetOutputDto> CreateAsync(TCreateInput input);
        Task DeleteAsync(Guid id);
        Task<TGetOutputDto> GetAsync(Guid id);
        Task<PagedResultDto<TGetOutputDto>> GetListAsync(TGetListInput input);
        Task<TGetOutputDto> UpdateAsync(Guid id, TUpdateInput input);
        //todo:merge to update
        //Task<TGetOutputDto> MoveAsync(TMoveInput input);
    }
}