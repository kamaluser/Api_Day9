﻿using Course.Service.Dtos.StudentDtos;
using Course.Service.Exceptions;
using Course.Service.Services.Implementations;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
        {
            _studentService = studentService;
            _logger = logger;
        }

        [HttpPost("")]
        public ActionResult Create(StudentCreateDto createDto)
        {
            _logger.LogInformation("Create request received with Email: {Email}", createDto.Email);
            return StatusCode(201, new { id = _studentService.Create(createDto) });
        }

        [HttpGet("")]
        public ActionResult<List<StudentGetDto>> GetAll(string? search = null, int page = 1,int size = 10)
        {
            _logger.LogInformation("GetAll request received");
            return Ok(_studentService.GetAllByPage(search, page,size));
        }

        [HttpGet("{id}")]
        public ActionResult<StudentGetDto> GetById(int id)
        {
            _logger.LogInformation("GetById request received for Id: {Id}", id);
            return Ok(_studentService.GetById(id));
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, StudentEditDto editDto)
        {
            _logger.LogInformation("Edit request received for Id: {Id}", id);
            _studentService.Edit(id, editDto);
            _logger.LogInformation("Edit request processed successfully for Id: {Id}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _logger.LogInformation("Delete request received for Id: {Id}", id);
            _studentService.Delete(id);
            _logger.LogInformation("Delete request processed successfully for Id: {Id}", id);
            return NoContent();
        }
    }
}
