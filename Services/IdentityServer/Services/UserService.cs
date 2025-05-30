using FluentValidation;
using IdentityServer.Context;
using IdentityServer.DTOs;
using IdentityServer.Exceptions;
using IdentityServer.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Services;

public interface IUserService
{
    Task<UserShowDto> CreateUser(CreateUserDto request);
    Task<UserShowDto> UpdateUser(string userId, UpdateUserDto requet);
    Task<UserDetailedDto> GetUserByEmailAsync(string email);
    Task<UserDetailedDto> GetUserByIdAsync(string id);
    Task<IdentityRole> GetUserRole(string userId);
    Task<List<UserShowDto>> GetUsersAync();
    Task<bool> ChangePassword(ChangePasswordDto request);
}
public class UserService : IUserService
{
    private readonly IdentityDbContext _context;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;
    public UserService(IdentityDbContext context, IValidator<CreateUserDto> createUserValidator, IValidator<UpdateUserDto> updateUserValidator)
    {
        _context = context;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
    }

    public async Task<bool> ChangePassword(ChangePasswordDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null)
        {
            throw new NotFoundException("İstifadəçi tapılmadı");
        }
        var pass = BCrypt.Net.BCrypt.Verify(request.OldPassword, user.HashedPassword);
        if (!pass)
        {
            throw new ValidationException("Köhnə şifrə yanlışdır");
        }
        user.ChangePassword(request.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserShowDto> CreateUser(CreateUserDto request)
    {
        var valResult = _createUserValidator.Validate(request);
        if (!valResult.IsValid)
        {
            throw new ValidationException(valResult.Errors);
        }

        var user = new IdentityUser(request.FirstName, request.LastName, request.Email, request.PhoneNumber, request.Password, request.CompanyId, request.WarehouseId);
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == user.NormalizedEmail);
        if (existingUser != null)
        {
            throw new ConflictException("Bu email istifadə olunub");
        }
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        var userDto = new UserShowDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CompanyId = user.CompanyId,
            WarehouseId = user.WarehouseId,
            Roles = user.Roles.Select(r => r.Role.RoleName).ToList()
        };
        return userDto;
    }

    public async Task<UserDetailedDto> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.Roles.Where(ur => !ur.Revoked.HasValue))
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == email.Trim().ToLower());
        if (user == null)
        {
            throw new NotFoundException("İstifadəçi tapılmadı");
        }
        var userDto = new UserDetailedDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            HashedPassword = user.HashedPassword,
            CompanyId = user.CompanyId,
            WarehouseId = user.WarehouseId,
            Roles = user.Roles.Select(r => r.Role.RoleName).ToList()
        };
        return userDto;
    }

    public async Task<UserDetailedDto> GetUserByIdAsync(string id)
    {
        var user = await _context.Users
            .Include(u => u.Roles.Where(ur => !ur.Revoked.HasValue))
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new NotFoundException("İstifadəçi tapılmadı");
        }
        var userDto = new UserDetailedDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            HashedPassword = user.HashedPassword,
            CompanyId = user.CompanyId,
            WarehouseId = user.WarehouseId,
            Roles = user.Roles.Select(r => r.Role.RoleName).ToList()
        };
        return userDto;
    }

    public async Task<IdentityRole> GetUserRole(string userId)
    {
        var userRole = await _context.UserRoles
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == userId);
        if (userRole == null)
        {
            throw new Exception("İstifadəçinin rolu tapılmadı");
        }
        return userRole!.Role!;
    }

    public async Task<List<UserShowDto>> GetUsersAync()
    {
        var users = await _context.Users
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();

        return users.Select(u => new UserShowDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            CompanyId = u.CompanyId,
            WarehouseId = u.WarehouseId,
            Roles = u.Roles.Select(r => r.Role.RoleName).ToList()
        }).ToList();
    }

    public async Task<UserShowDto> UpdateUser(string userId, UpdateUserDto request)
    {
        var valResult = _updateUserValidator.Validate(request);
        if (!valResult.IsValid)
        {
            throw new ValidationException(valResult.Errors);
        }
        if(userId != request.UserId)
        {
            throw new Exception("Göndərilən Id dəyəri uyğun deyil");
        }
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            throw new Exception("İstifadəçi tapılmadı");
        }
        else
        {
            if (user.Email != request.Email)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.NormalizedEmail == request.Email.Trim().ToLower());
                if (existingUser != null)
                {
                    throw new Exception($"Dəyişdirmək istədiyiniz email istifadə olunub");
                }
            }
            user.Update(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
            await _context.SaveChangesAsync();
            var userDto = new UserShowDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = user.Roles.Select(r => r.Role!.RoleName).ToList()
            };
            return userDto;
        }
    }
}
