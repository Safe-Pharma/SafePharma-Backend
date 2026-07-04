using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafePharma.BLL;
using SafePharma.Common;
using System.Security.Claims;

namespace SafePharma.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLanguageController : ControllerBase
    {
        private readonly IUserLanguageManager _manager;
        private readonly IValidator<UpdateLanguageDto> _validator;

        public UserLanguageController(IValidator<UpdateLanguageDto> validator, IUserLanguageManager manager)
        {
            _validator = validator;
            _manager = manager;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLanguage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId is null) return Ok(new { language = "en" });
            // need to be commented

            var result = await _manager.GetLanguageAsync(userId);
            return Ok(result);
        }

        [HttpPut]
        [Authorize]

        public async Task<IActionResult> UpdateLanguage([FromBody] UpdateLanguageDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => new Error
                        {
                            ErrorCode = e.ErrorCode,
                            ErrorMessage = e.ErrorMessage
                        }).ToList()
                    );
                return BadRequest(GeneralResult.FailResult(errors));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (userId is null) return Ok(new { language = "en" });  // need to be commented

            var result = await _manager.UpdateLanguageAsync(userId, dto);

            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
