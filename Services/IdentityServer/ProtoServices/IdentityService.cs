using Grpc.Core;
using IdentityServer.DTOs.User;
using IdentityServer.Models;
using IdentityServer.Protos;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shared.ResultTypes;
using IdentityServer.Services;
namespace IdentityServer.ProtoServices
{
    
    public class IdentityService : Identity.IdentityBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public override async Task<GetEmployeesResponse> GetEmployees(GetEmployeesRequest request, ServerCallContext context)
        {
            var response = new GetEmployeesResponse();
            List<ApplicationUser> users = new List<ApplicationUser>();
            if (request.CompanyId != "Sandal")
            {
                users = await _userManager.Users.Where(x => x.CompanyId == request.CompanyId).ToListAsync();
            }
            else
            {
                users = await _userManager.Users.ToListAsync();
            }
            try
            {
                foreach (var user in users)
                {
                    if (user == null) continue;
                    var roles = await _userManager.GetRolesAsync(user);
                    response.Employees.Add(new Employee
                    {
                        Id = user.Id,
                        Name = user.UserName,
                        Email = user.Email,
                        CompanyId = user.CompanyId,
                        WarehouseId = user.WarehouseId ?? "N/A",
                        Role = roles.FirstOrDefault() ?? "Unknown"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC GetEmployees Error: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            return response;
        }

        public override async Task<GetEmployeeResponse> GetEmployee(GetEmployeeRequest request, ServerCallContext context)
        {
            var response = new GetEmployeeResponse();
            try
            {
                var user = await _userManager.FindByIdAsync(request.Id);
                if (user == null)
                {
                    response.Success = false;
                    response.Message.Add("User not found");
                }
                else
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    response.Employee = new Employee
                    {
                        Id = user.Id,
                        Name = user.UserName,
                        Email = user.Email,
                        CompanyId = user.CompanyId,
                        WarehouseId = user.WarehouseId ?? "N/A",
                        Role = roles.FirstOrDefault() ?? "Unknown"
                    };
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"gRPC GetEmployee Error: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
            return response;
        }

        public override async Task<CreateEmployeeResponse> CreateEmployee(CreateEmployeeRequest request, ServerCallContext context)
        {
            var response = new CreateEmployeeResponse();
            var employee = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Name,
                Email = request.Email,
                CompanyId = request.CompanyId,
                WarehouseId = request.WarehouseId == "" ? "N/A" : request.WarehouseId
            };
            try
            {
                var result = await _userManager.CreateAsync(employee, request.Password);
                if (result.Succeeded)
                {
                    response.Success = true;
                    response.Message.AddRange(result.Errors.Select(x => x.Description).ToList());
                }
                else
                {
                    response.Success = false;
                    response.Message.AddRange(result.Errors.Select(x => x.Description).ToList());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC AddEmployee Error: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
            return response;
        }
    }
}
