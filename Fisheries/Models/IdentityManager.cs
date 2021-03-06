﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Fisheries.Models
{
    public class IdentityManager
{
    // Swap ApplicationRole for IdentityRole:
    RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(
        new RoleStore<ApplicationRole>(new ApplicationDbContext()));
  
    UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(
        new UserStore<ApplicationUser>(new ApplicationDbContext()));
  
    ApplicationDbContext _db = new ApplicationDbContext();
  
  
    public bool RoleExists(string name)
    {
        return _roleManager.RoleExists(name);
    }
  
  
    public bool CreateRole(string name, string description = "")
    {
        // Swap ApplicationRole for IdentityRole:
        var idResult = _roleManager.Create(new ApplicationRole(name, description));
        return idResult.Succeeded;
    }
  
  
    public bool CreateUser(ApplicationUser user, string password)
    {
        var idResult = _userManager.Create(user, password);
        return idResult.Succeeded;
    }
  
  
    public bool AddUserToRole(string userId, string roleName)
    {
        var idResult = _userManager.AddToRole(userId, roleName);
        return idResult.Succeeded;
    }
  
  
    public void ClearUserRoles(string userId)
    {
        var user = _userManager.FindById(userId);
        var currentRoles = new List<IdentityUserRole>();
  
        currentRoles.AddRange(user.Roles);
        foreach (var role in currentRoles)
        {
                _userManager.RemoveFromRole(userId, role.RoleId);
        }
    }
}
}