using Microsoft.AspNetCore.Mvc;
using VillaApi.Models;
using VillaApi.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace VillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        protected readonly ApiResponse _apiResponse;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<VillaController> _logger;

        public AuthController(IUserRepository userRepository, ILogger<VillaController> logger)
        {
            this._userRepository = userRepository;
            _logger = logger;
            _apiResponse = new();
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel signUpModel)
        {
            try
            {
                var result = await _userRepository.SignUpAsync(signUpModel);
                result.Errors.ToList().ForEach(e => _apiResponse.PushErrors(e.Description));
                if(result.Succeeded){
                    _apiResponse.Success(result, HttpStatusCode.OK);
                    return Ok(_apiResponse.Success("User created successfully"));
                }else{
                    _apiResponse.Fail(HttpStatusCode.Unauthorized);
                    return Unauthorized(_apiResponse);
                }
            }
            catch (Exception ex)
            {
                _apiResponse.PushErrors(ex.Message);
                _apiResponse.Fail(HttpStatusCode.InternalServerError);
                return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
            }
        }
        
        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel signInModel)
        {
            try
            {
                var result = await _userRepository.SignInAsync(signInModel);
                
                if(!string.IsNullOrEmpty(result)){
                    _apiResponse.Success(result, HttpStatusCode.OK);
                    return Ok(_apiResponse);
                }else{
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                _apiResponse.PushErrors(ex.Message);
                _apiResponse.Fail(HttpStatusCode.InternalServerError);
                return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
            }
        }
        [Authorize]
        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOutVip()
        {
            var httpContext = HttpContext;
           
            httpContext.Request.Headers.TryGetValue("Authorization", out var token);
            System.Console.WriteLine(token);

            try
            {
                await _userRepository.SignOutAsync();
                _apiResponse.Success("User signed out successfully", HttpStatusCode.OK);
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.PushErrors(ex.Message);
                _apiResponse.Fail(HttpStatusCode.InternalServerError);
                return StatusCode((int)_apiResponse.StatusCode, _apiResponse);
            }
        }
    }
}