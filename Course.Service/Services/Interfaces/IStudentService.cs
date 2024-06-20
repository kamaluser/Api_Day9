using Course.Service.Dtos;
using Course.Service.Dtos.StudentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Services.Interfaces
{
    public interface IStudentService
    {
        int Create(StudentCreateDto dto);
        List<StudentGetDto> GetAll(string? search = null);
        PaginatedList<StudentGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        StudentGetDto GetById(int id);
        void Edit(int id, StudentEditDto dto);
        void Delete(int id);
    }
}
