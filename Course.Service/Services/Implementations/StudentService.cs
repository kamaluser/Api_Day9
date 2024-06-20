using AutoMapper;
using Course.Core.Entities;
using Course.Data;
using Course.Data.Repostories.Implementations;
using Course.Data.Repostories.Interfaces;
using Course.Service.Dtos;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Course.Service.Mappers;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Group = Course.Core.Entities.Group;

namespace Course.Service.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<GroupService> _logger;
        private readonly IMapper _mapper;

        public StudentService(IGroupRepository groupRepository,IStudentRepository repository, IWebHostEnvironment environment, ILogger<GroupService> logger, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _studentRepository = repository;
            _environment = environment;
            _logger = logger;
            _mapper = mapper;
        }
        public int Create(StudentCreateDto dto)
        {
            _logger.LogInformation("Create method called with Email: {Email}", dto.Email);
            Group group = _groupRepository.Get(g => g.Id == dto.GroupId && !g.IsDeleted, "Students");

            if (group == null)
            {
                _logger.LogWarning("Group not found by given GroupId", dto.GroupId);
                throw new RestException(StatusCodes.Status404NotFound, "GroupId", "Group not found by given GroupId");
            }

            if (group.Limit <= group.Students.Count)
            {
                _logger.LogWarning("Group is full.");
                throw new RestException(StatusCodes.Status400BadRequest, "Group is full");
            }

            if (_studentRepository.Exists(s => s.Email.ToUpper() == dto.Email.ToUpper() && !s.IsDeleted))
            {
                _logger.LogWarning("Student with Email: {Email} already exists", dto.Email);
                throw new RestException(StatusCodes.Status400BadRequest, "Email", "Student already exists by given Email");
            }

            string file = null;
            if (dto.File != null)
            {
                file = SaveFile(dto.File);
            }

            var student = _mapper.Map<Student>(dto);
            student.Photo = file;

            _studentRepository.Add(student);
            _studentRepository.Save();

            _logger.LogInformation("Student created with Id: {Id}", student.Id);
            return student.Id;
        }

        public List<StudentGetDto> GetAll(string? search = null)
        {
            _logger.LogInformation("GetAll method called");

            var students = _studentRepository.GetAll(x => (search == null || x.FullName.Contains(search)) && !x.IsDeleted, "Group").ToList();

            var studentDtos = students.Select(student =>
            {
                var dto = _mapper.Map<StudentGetDto>(student);
                return dto;
            }).ToList();

            _logger.LogInformation("GetAll method completed");

            return studentDtos;
        }

        public StudentGetDto GetById(int id)
        {
            _logger.LogInformation("GetById method called for Id: {Id}", id);
            var student = _studentRepository.Get(s => s.Id == id && !s.IsDeleted, "Group");
            if (student == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Student with {id} ID not found.");
    
            _logger.LogInformation("GetById method completed for Id: {Id}", id);

            StudentGetDto dto = _mapper.Map<StudentGetDto>(student);
            return dto;
        }
        public void Edit(int id, StudentEditDto dto)
        {
            _logger.LogInformation("Edit method called for Id: {Id}", id);

            var student = _studentRepository.Get(s => s.Id == id && !s.IsDeleted);
            if (student == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Student with {id} ID not found.");

            Group group = _groupRepository.Get(g => g.Id == dto.GroupId && !g.IsDeleted, "Students");

            if (group == null)
                throw new RestException(StatusCodes.Status404NotFound,$"Group with ID {dto.GroupId} not found.");

            if (group.Limit <= group.Students.Count)
                throw new RestException(StatusCodes.Status400BadRequest,"Group is full!");

            if (dto.File != null)
            {
                string path = SaveFile(dto.File);
                student.Photo = path;
            }

            Mapper<StudentEditDto, Student>.Map(dto, student);

            _studentRepository.Save();
            _logger.LogInformation("Group edited successfully for Id: {Id}", id);
        }

        public void Delete(int id)
        {
            _logger.LogInformation("Delete method called for Id: {Id}", id);

            var student = _studentRepository.Get(s => s.Id == id && !s.IsDeleted);
            if (student == null)
                throw new RestException(StatusCodes.Status404NotFound, $"Student with {id} ID not found.");

            student.IsDeleted = true;
            _studentRepository.Save();

            _logger.LogInformation("Group deleted successfully for Id: {Id}", id);
        }

        private string SaveFile(IFormFile file)
        {
            string uploadDir = Path.Combine(_environment.WebRootPath, "uploads/student");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return fileName;
        }

        public PaginatedList<StudentGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
            var query = _studentRepository.GetAll(x => (search == null || x.FullName.Contains(search)) && !x.IsDeleted, "Group");
            var paginated = PaginatedList<Student>.Create(query, page, size);
            return new PaginatedList<StudentGetDto>(_mapper.Map<List<StudentGetDto>>(paginated.Items), paginated.TotalPages, page, size);
        }
    }
}
