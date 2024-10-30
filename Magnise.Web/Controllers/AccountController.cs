using AutoMapper;
using Contracts.Dtos;
using Contracts.Services;
using Magnise.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Magnise.Web.Controllers;

[ApiController]
[Route("api/user")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;

    public AccountController(
        IAccountService accountService,
        IMapper mapper)
    {
        _accountService = accountService;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromForm] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var dto = _mapper.Map<LoginDto>(model);

        var token = await _accountService.GetTokenAsync(dto);

        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        HttpContext.Session.SetString("AccessToken", token);

        return Ok();
    }
}
