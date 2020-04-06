using Mastermind.Api.Models;
using Mastermind.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Mastermind.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserService UserService { get; }

        public UserController(UserService userService) => UserService = userService;

        [HttpPost("api/user/register")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> RegisterAsync([FromBody] UserAuthModel userAuthModel)
        {
            await UserService.HttpCookieSignInAsync(await UserService.RegisterAsync(userAuthModel.Username, userAuthModel.Password));
            return NoContent();
        }

        [HttpPost("api/user/login")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> LoginAsync([FromBody] UserAuthModel userAuthModel)
        {
            var id = await UserService.CheckPasswordAsync(userAuthModel.Username, userAuthModel.Password);
            if (id != null)
            {
                await UserService.HttpCookieSignInAsync(id.Value);
                return NoContent();
            }
            return BadRequest();
        }
    }
}