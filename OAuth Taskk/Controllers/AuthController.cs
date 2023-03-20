using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OAuth_Taskk.Dto;
using OAuth_Taskk.Helpers;
using OAuth_Taskk.Repos.AuthRepos;

namespace OAuth_Taskk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _repo;
        private readonly UserManager<IdentityUser> _usermanager;

        public AuthController(IAuthRepo repo, UserManager<IdentityUser> usermanager)
        {
            _repo = repo;
            _usermanager = usermanager;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Respons<AuthDTO>>> RegisterAsync([FromBody] RegisterDTO dTO)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(new Respons<AuthDTO> { Error = ModelState.ToString() });
                }

                var res = await _repo.RegisterAsync(dTO);

                if (!res.Data.IsAuthenticated)
                {
                    return BadRequest(res);
                }

                if (res.Error == null)
                {
                    return Ok(res);
                }

                return BadRequest(res);
            }
            catch (Exception ex)
            {

                return BadRequest(new Respons<AuthDTO> { Error = ex.Message });
            }
        }


        [HttpPost("GetToken")]
        public async Task<ActionResult<Respons<AuthDTO>>> GetTokenAsync([FromBody] TokenRequestDTO dTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Respons<AuthDTO> { Error = ModelState.ToString() });
                }

                var res = await _repo.GetToken(dTO);

                if (!res.Data.IsAuthenticated)
                {
                    return BadRequest(res);
                }

                if (res.Error == null)
                {
                    return Ok(res);
                }

                return BadRequest(res);
            }
            catch (Exception ex)
            {

                return BadRequest(new Respons<AuthDTO> { Error = ex.Message });
            }
        }



        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLoginAsynck([FromBody] ExternalAuthDto externalAuth)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Respons<AuthDTO> { Error = ModelState.ToString() });
                }

                var res = await _repo.ExtrnalLogin(externalAuth);

                if (!res.Data.IsAuthenticated)
                {
                    return BadRequest(res);
                }

                if (res.Error == null)
                {
                    return Ok(res);
                }

                return BadRequest(res);
            }
            catch (Exception ex)
            {

                return BadRequest(new Respons<AuthDTO> { Error = ex.Message });
            }
        }
    }
}
