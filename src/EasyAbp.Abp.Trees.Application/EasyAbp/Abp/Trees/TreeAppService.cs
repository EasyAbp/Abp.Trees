using EasyAbp.Abp.Trees.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Application.Services;

namespace EasyAbp.Abp.Trees
{
    //todo: merge to abstract class TreesAppService
    public class TreeAppService<TEntity, TGetOutputDto, TGetListInput, TCreateInput, TUpdateInput, TMoveInput>
        : CrudAppService<TEntity, TGetOutputDto, Guid, TGetListInput, TCreateInput, TUpdateInput>,
        ITreeAppService<TGetOutputDto, TGetListInput, TCreateInput, TUpdateInput, TMoveInput> where TEntity : class, ITree<TEntity>, IEntity<Guid>, new()
        where TGetOutputDto : ITreeDto
        where TGetListInput : IGetListInput
        where TCreateInput : ICreateInput
        where TUpdateInput : IUpdateInput
        where TMoveInput : IMoveInput
    {
        protected ITreeRepository<TEntity> TreeRepository { get; }

        protected virtual string MovePolicyName { get; }

        public TreeAppService(
            ITreeRepository<TEntity> treeRepository
            )
            : base(treeRepository)
        {
            this.TreeRepository = treeRepository;
        }
        public virtual async Task<TGetOutputDto> MoveAsync(TMoveInput input)
        {
            var entity = await this.TreeRepository.GetAsync(input.Id);

            await CheckUpdatePolicyAsync();

            await TreeRepository.MoveAsync(entity, input.NewParentId);

            return MapToGetOutputDto(await this.TreeRepository.GetAsync(input.Id));
        }
        protected virtual async Task CheckMovePolicyAsync()
        {
            await CheckPolicyAsync(MovePolicyName);
        }
        protected virtual PagedResultDto<TDto> CreatePagedResultDto<TDto>((int TotalCount, System.Collections.Generic.List<TEntity> Entities) queryResult)
        {
            return new PagedResultDto<TDto>(
                queryResult.TotalCount,
                queryResult.Entities.Select(x => MapToDto<TDto>(x)).ToList());
        }

        protected virtual TDto MapToDto<TDto>(TEntity entity)
        {
            return ObjectMapper.Map<TEntity, TDto>(entity);
        }

    }
}
