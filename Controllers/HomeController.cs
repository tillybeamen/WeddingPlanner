﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Mvc.Filters;


namespace WeddingPlanner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext db;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    public IActionResult Index()
    {
        return View("Index");
    }

    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }
        PasswordHasher<User> hashedPW = new PasswordHasher<User>();
        newUser.Password = hashedPW.HashPassword(newUser, newUser.Password);
        db.Users.Add(newUser);
        db.SaveChanges();

        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        HttpContext.Session.SetString("FirstName", newUser.FirstName);
        return RedirectToAction("Weddings", "Wedding");
    }

    [HttpPost("login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if (!ModelState.IsValid)
        {
            return Index();
        }
        User? dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);
        if (dbUser == null)
        {
            ModelState.AddModelError("LoginEmail", "does not match");
            return Index();
        }
        PasswordHasher<LoginUser> hashedPW = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompare = hashedPW.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);
        if(pwCompare == 0)
        {
            ModelState.AddModelError("LoginPassword", "does not match");
            return Index();
        }
        HttpContext.Session.SetInt32("UUID", dbUser.UserId);
        HttpContext.Session.SetString("FirstName", dbUser.FirstName);
        // HttpContext.Session.SetString("FirstName", dbUser.FirstName);
        // string? FirstName = dbUser.FirstName;
        return RedirectToAction("Weddings", "Wedding");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class SessionCheckAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Find the session, but remember it may be null so we need int?
        int? userId = context.HttpContext.Session.GetInt32("UUID");
        // Check to see if we got back null
        if(userId == null)
        {
            // Redirect to the Index page if there was nothing in session
            // "Home" here is referring to "HomeController", you can use any controller that is appropriate here
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}