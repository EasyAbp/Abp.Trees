using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAbp.Abp.Trees.Dtos
{
    public interface ICreateInput
    {
        Guid? ParentId { get; set; }

        string DisplayName { get; set; }
    }
}
