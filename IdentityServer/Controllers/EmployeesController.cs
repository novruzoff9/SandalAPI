using IdentityServer.DTOs.User;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeesController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(string companyId)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            if(companyId != "Sandal")
            {
                users = await _userManager.Users.Where(x => x.CompanyId == companyId).ToListAsync();
            }
            else
            {
                users = await _userManager.Users.ToListAsync();
            }
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

        [HttpPost]
        public async Task<IActionResult> AddEmployee(CreateUserDto request, string companyId)
        {
            var employee = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.UserName,
                Email = request.Email,
                CompanyId = companyId
            };

            var result = await _userManager.CreateAsync(employee, request.Password);

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
