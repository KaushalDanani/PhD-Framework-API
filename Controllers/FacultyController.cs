using Backend.Data;
using Backend.DTOs;
using Backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFacultyRepository _faultyRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public FacultyController(ApplicationDbContext context, IFacultyRepository facultyRepository, IDepartmentRepository departmentRepository)
        {
            _context = context;
            _faultyRepository = facultyRepository;
            _departmentRepository = departmentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacultyResponseDto>>> GetFaculties()
        {
            try
            {
                var faculties = await _faultyRepository.GetAllFacultyAsync();
                return Ok(faculties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving faculties.");
            }
        }

        [HttpGet("{facultyId}/departments")]
        public async Task<ActionResult<DepartmentResponseDto>> GetDepartmentsByFaculty(int facultyId)
        {
            if (facultyId <= 0)
                return BadRequest(new { message = "Invalid Faculty" });

            try
            {
                var facultyExist = await _context.Faculties.AnyAsync(f => f.Id == facultyId);
                if (!facultyExist)
                {
                    return NotFound(new { message = "Faculty not found." });
                }

                var departments = await _departmentRepository.GetDepartmentListByFacultyAsync(facultyId);
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving departments.");
            }
        }
    }
}
