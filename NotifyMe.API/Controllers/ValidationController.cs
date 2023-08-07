using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.API.Controllers;

public class ValidationController : Controller
{
    private DatabaseContext _db;

    public ValidationController(DatabaseContext db)
    {
        _db = db;
    }

    [HttpGet]
    public bool CheckUserName(int? id, string userName)
    {
        if (id is null || id == 0)
        {
            return !_db.Users.Any(p=> p.UserName == userName);
        }
        return _db.Users.Any(p=> p.UserName == userName);
    }
        
    [HttpGet]
    public bool IsExist(string userName)
    {
        return _db.Users.Any(p=> p.UserName == userName);
    }
        
    [HttpGet]
    public bool CheckEmailAddress(string email)
    {
        if (!email.IsNullOrEmpty())
        {
            return !_db.Users.Any(p=> p.Email == email);
        }
        return _db.Users.Any(p=> p.Email == email);
    }
        
}