using Backend.CustomExceptions;
using Backend.Interfaces;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhdTitleController : ControllerBase
    {
        private readonly IPhDTitleService _phdTitleService;

        public PhdTitleController(IPhDTitleService phdTitleService)
        {
            _phdTitleService = phdTitleService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> IsPhDTitleAvailable()
        {
            try
            {
                var result = await _phdTitleService.IsPhDTitleRegisterAsync();
                return Ok(new { isRegister = result });
            }
            catch (InvalidOperationException ioe)
            {
                return BadRequest(new { message = ioe.Message });
            }
            catch (UserNotFoundException ue)
            {
                return Unauthorized(new { message = ue.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
