using Course.Service.Dtos;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Exceptions;
using Course.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupsController> _logger;

        public GroupsController(IGroupService groupService, ILogger<GroupsController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        [HttpPost("")]
        public ActionResult Create(GroupCreateDto createDto)
        {
            _logger.LogInformation("Create request received with No: {No}", createDto.No);
            var id = _groupService.Create(createDto);
            _logger.LogInformation("Group created with Id: {Id}", id);
            return StatusCode(201, new { id });
        }

        [HttpGet("")]
        public ActionResult<PaginatedList<GroupGetDto>> GetAll(string? search = null, int page = 1, int size = 10)
        {
            _logger.LogInformation("GetAll request processed successfully");
            return StatusCode(200, _groupService.GetAllByPage(search, page, size));
        }

        [HttpGet("{id}")]
        public ActionResult<GroupGetDto> GetById(int id)
        {
            _logger.LogInformation("GetById request received for Id: {Id}", id);
            var result = _groupService.GetById(id);
            _logger.LogInformation("GetById request processed successfully for Id: {Id}", id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, GroupEditDto editDto)
        {
            _logger.LogInformation("Edit request received for Id: {Id}", id);
            _groupService.Edit(id, editDto);
            _logger.LogInformation("Edit request processed successfully for Id: {Id}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _logger.LogInformation("Delete request received for Id: {Id}", id);
            _groupService.Delete(id);
            _logger.LogInformation("Delete request processed successfully for Id: {Id}", id);
            return NoContent();
        }
    }
}
