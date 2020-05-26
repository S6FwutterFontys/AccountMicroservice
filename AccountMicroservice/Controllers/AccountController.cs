using System;
using System.Threading.Tasks;
using AccountMicroservice.Exceptions;
using AccountMicroservice.Models;
using AccountMicroservice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountModel createAccountModel)
        {
            try
            {
                return Ok(await _accountService.CreateAccount(createAccountModel));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(Guid id)
        {
            var account = await _accountService.GetAccount(id);
            return Ok(account);
        }

        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, UpdateAccountModel updateAccountModel)
        {
            try
            {
                return Ok(await _accountService.UpdateAccount(id, updateAccountModel));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [AllowAnonymous]
        [HttpPut("UpdatePassword/{id}")]
        public async Task<IActionResult> UpdatePassword(Guid id, [FromBody]UpdatePasswordModel updatedPasswordModel)
        {
            try
            {
                return Ok(await _accountService.UpdatePassword(id, updatedPasswordModel));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            await _accountService.DeleteAccount(id);
            return Ok();
        }
        
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                return Ok(await _accountService.Login(loginModel));
            }
            catch (EmailNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // [AllowAnonymous]
        // [HttpPost("users/authenticate")]
        // public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        // {
        //     try
        //     {
        //         return Ok(await _accountService.Login(loginModel));
        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }
    }
}