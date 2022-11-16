using DemoJWT.Interfaces;
using DemoShared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromForm]LoginRequestDTO request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var resultToken = await _userRepository.Authenticate(request);
            if(string.IsNullOrEmpty(resultToken))
            {
                return BadRequest("Username or password is incorrect.");
            }
            return Ok(new { token = resultToken });
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userRepository.Register(request);
            if (!result)
            {
                return BadRequest("Register is unsuccessful");
            }
            return Ok();
        }
    }
}
