using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeachPortal.Application.DTOs.Auth;
using TeachPortal.Application.Services;

namespace TeachPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;
    private readonly ILogger<TeachersController> _logger;

    public TeachersController(ITeacherService teacherService, ILogger<TeachersController> logger)
    {
        _teacherService = teacherService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetAllTeachers()
    {
        var teachers = await _teacherService.GetAllTeachersAsync();
        return Ok(teachers);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TeacherDto>> GetTeacher(Guid id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);

        if (teacher == null)
        {
            return NotFound(new { message = "Teacher not found" });
        }

        return Ok(teacher);
    }

    [HttpGet("me")]
    public async Task<ActionResult<TeacherDto>> GetCurrentTeacher()
    {
        var teacherIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(teacherIdClaim) || !Guid.TryParse(teacherIdClaim, out var teacherId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        var teacher = await _teacherService.GetTeacherByIdAsync(teacherId);

        if (teacher == null)
        {
            return NotFound(new { message = "Teacher not found" });
        }

        return Ok(teacher);
    }

    [HttpGet("{id:guid}/students")]
    public async Task<ActionResult<TeacherDto>> GetTeacherWithStudents(Guid id)
    {
        var teacher = await _teacherService.GetTeacherWithStudentsAsync(id);

        if (teacher == null)
        {
            return NotFound(new { message = "Teacher not found" });
        }

        return Ok(teacher);
    }
}