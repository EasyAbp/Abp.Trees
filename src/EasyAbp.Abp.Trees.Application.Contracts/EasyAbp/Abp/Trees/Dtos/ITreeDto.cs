using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Abp.Trees.Dtos
{
    public interface ITreeDto : IEntityDto<Guid>
    {
        Guid? ParentId { get; set; }

        string Code { get; set; }

        string DisplayName { get; set; }
    }
}