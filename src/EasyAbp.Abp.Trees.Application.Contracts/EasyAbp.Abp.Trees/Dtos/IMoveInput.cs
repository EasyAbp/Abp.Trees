using System;
using System.ComponentModel.DataAnnotations;

namespace EasyAbp.Abp.Trees.Dtos
{
    public interface IMoveInput
    {
        Guid Id { get; set; }

        Guid? NewParentId { get; set; }
    }
}
