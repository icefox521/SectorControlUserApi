using Microsoft.AspNetCore.Mvc;
using SectorControlApi.Services;
using SectorControlApi.Models.Requests;

namespace SectorControlApi.Controllers
{
    /// <summary>
    /// Controller for handling basic operations with users such as registration and verification.
    /// </summary>
    [Route("users/[action]")]
    public class UserController : Controller
    {
        private readonly IPasswordService _passwordService;
        private readonly IUserService _userService;
        private readonly ILogService _logService;

        public UserController(IPasswordService passwordService, IUserService userService, ILogService logService)
        {
            _passwordService = passwordService;
            _userService = userService;
            _logService = logService;
        }

        /// <summary>
        /// Creates new player in DB.
        /// </summary>
        /// <returns>
        /// True if user was successfuly created.
        /// </returns>
        [HttpPost]
        [ActionName("Register")]
        public async Task<bool> RegisterAsync([FromBody]RegisterRequestModel request)
        {
            bool isSuccess = false;

            if (!TryValidateModel(request))
            {
                return isSuccess;
            }

            if (!string.IsNullOrEmpty(request?.Name) && !string.IsNullOrEmpty(request?.Password) && !string.IsNullOrEmpty(request?.Email))
            {
                byte[] salt = _passwordService.CreateSalt();
                byte[] pswdHash = await _passwordService.HashPasswordAsync(request.Password, salt);
                string hashString = Convert.ToBase64String(pswdHash);
                string saltString = Convert.ToBase64String(salt);

                try
                {
                    await _userService.CreateUserInDatabaseAsync(request.Name, request.Email, saltString, hashString);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    _logService.Error("Unexpected error. " + ex);
                }
            }
            else
            {
                _logService.Error($"Error, register request was empty");
            }
            return isSuccess;
        }

        /// <summary>
        /// Verifies login details for player login.
        /// </summary>
        /// <returns>
        /// Access token. TODO: add timestamp for expiration and createdon.
        /// </returns>
        [HttpPost]
        [ActionName("Verify")]
        public async Task<IActionResult> VerifyUserAsync([FromBody] VerifyRequestModel request)
        {
            string? token = null;

            if (!TryValidateModel(request))
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(request?.Email) && !string.IsNullOrEmpty(request?.Password))
            {
                try
                {
                    var taskResult = await _userService.AuthenticateAsync(request.Email, request.Password);
                    token = taskResult?.ToString();
                }
                catch(Exception ex)
                {
                    _logService.Error("Unexpected error. " + ex);
                }
            }
            else
            {
                _logService.Error($"Error, verify request was empty");
            }

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }
    }
}
