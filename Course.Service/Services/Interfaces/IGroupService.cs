using Course.Service.Dtos;
using Course.Service.Dtos.GroupDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Services.Interfaces
{
    public interface IGroupService
    {
        int Create(GroupCreateDto createDto);
        PaginatedList<GroupGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        void Edit(int id, GroupEditDto editDto);
        void Delete(int id);
        GroupGetDto GetById(int id);
    }
}
