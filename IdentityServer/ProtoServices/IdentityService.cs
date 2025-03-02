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
                throw new RpcException(new Status(StatusCode.Internal, ex.Message)); // Hata detaylarını gRPC cevabına ekliyoruz
            }

            return response;
        }
    }
}
