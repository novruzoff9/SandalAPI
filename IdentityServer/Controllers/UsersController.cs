using IdentityServer.DTOs.User;
using IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.Email,
                CompanyId = request.CompanyId
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Ok(new
                {
                    Errors = errors
                });
            }

            return Ok();
        }

        [HttpGet("WithToken")]
        public async Task<IActionResult> GetUser()
        {
            var user1 = User;
            var claims = user1.Claims;
            var userIdClaims = claims.FirstOrDefault(x => x.Type == "sub");

            if (userIdClaims == null)
            {
                return BadRequest("Id degeri tapilmadi");
            }

            var user = await _userManager.FindByIdAsync(userIdClaims.Value);

            if (user == null)
            {
                return BadRequest();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CompanyId = user.CompanyId,
                Roles = roles.ToList() ?? Array.Empty<string>().ToList()
            });
        }

        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userslist = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userslist.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    CompanyId = user.CompanyId,
                    WarehouseId = user.WarehouseId,
                    Roles = roles.ToList() ?? Array.Empty<string>().ToList()
                });
            }

            return Ok(userslist);
        }

        [HttpGet("Employees")]
        public async Task<IActionResult> GetEmployees(string companyId)
        {
            var users = await _userManager.Users.Where(x=>x.CompanyId == companyId).ToListAsync();
            var userslist = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userslist.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    CompanyId = user.CompanyId,
                    WarehouseId = user.WarehouseId,
                    Roles = roles.ToList() ?? Array.Empty<string>().ToList()
                });
            }

            return Ok(userslist);
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> CreateEmployee([FromBody] SignUpDto request)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.Email,
                //CompanyId = _identityService.GetCompanyId
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new
                {
                    Errors = errors
                });
            }

            return Ok();
        }

        [HttpPost("UpdateBranch")]
        public async Task<IActionResult> UpdateBranch(string userId, string branchId)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userId);
            user.WarehouseId = branchId;
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return Ok(new
                {
                    Errors = errors
                });
            }
            
            return Ok();
        }
    }
}
