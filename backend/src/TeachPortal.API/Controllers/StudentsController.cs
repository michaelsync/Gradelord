using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeachPortal.Application.DTOs.Students;
using TeachPortal.Application.Services;

namespace TeachPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService studentService, ILogger<StudentsController> logger)
    {
        _studentService = studentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetMyStudents()
    {
        var teacherIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(teacherIdClaim) || !Guid.TryParse(teacherIdClaim, out var teacherId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        var students = await _studentService.GetStudentsByTeacherAsync(teacherId);
        return Ok(students);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentDto>> GetStudent(Guid id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);

        if (student == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        // Verify the student belongs to the current teacher
        var teacherIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(teacherIdClaim) || !Guid.TryParse(teacherIdClaim, out var teacherId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        if (student.TeacherId != teacherId)
        {
            return Forbid("You can only access your own students");
        }

        return Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] CreateStudentRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var teacherIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(teacherIdClaim) || !Guid.TryParse(teacherIdClaim, out var teacherId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        var student = await _studentService.CreateStudentAsync(request, teacherId);
        _logger.LogInformation("Student {StudentId} created by teacher {TeacherId}", student.Id, teacherId);

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StudentDto>> UpdateStudent(Guid id, [FromBody] CreateStudentRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Verify the student exists and belongs to the current teacher
        var existingStudent = await _studentService.GetStudentByIdAsync(id);
        if (existingStudent == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        var teacherIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(teacherIdClaim) || !Guid.TryParse(teacherIdClaim, out var teacherId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        if (existingStudent.TeacherId != teacherId)
        {
            return Forbid("You can only update your own students");
        }

        var updatedStudent = await _studentService.UpdateStudentAsync(id, request);
        _logger.LogInformation("Student {StudentId} updated by teacher {TeacherId}", id, teacherId);

        return Ok(updatedStudent);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteStudent(Guid id)
    {
        // Verify the student exists and belongs to the current teacher
        var existingStudent = await _studentService.GetStudentByIdAsync(id);
        if (existingStudent == null)
        {
            return NotFound(new { message = "Student not found" });
        }

        var teacherIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(teacherIdClaim) || !Guid.TryParse(teacherIdClaim, out var teacherId))
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        if (existingStudent.TeacherId != teacherId)
        {
            return Forbid("You can only delete your own students");
        }

        await _studentService.DeleteStudentAsync(id);
        _logger.LogInformation("Student {StudentId} deleted by teacher {TeacherId}", id, teacherId);

        return NoContent();
    }
}