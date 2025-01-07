﻿namespace Organization.WebAPI.DTOs.User;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string CompanyId { get; set; }
    public string WarehouseId { get; set; }
    public string WarehouseName { get; set; }
    public List<string> Roles { get; set; }
}