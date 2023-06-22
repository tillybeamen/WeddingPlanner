using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;
[SessionCheck]
public class WeddingController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext db;

    public WeddingController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        db = context;
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("weddings")]
    public IActionResult Weddings()
    {
        // string? FirstName = HttpContext.Session.GetString("FirstName");
        // HttpContext.Session.SetString("FirstName", (string)FirstName);
        List<Wedding> allWeddings = db.Weddings.Include(r=> r.RSVPs).ToList();
        return View("Weddings", allWeddings);
    }

    [HttpGet("weddings/create")]
    public IActionResult AddWedding()
    {
        return View("WeddingForm");
    }

    [HttpPost("weddings/posted")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if (!ModelState.IsValid)
        {
            IEnumerable<ModelError> errors = ModelState.Values.SelectMany(v=>v.Errors);
            foreach(ModelError error in errors)
            {
                Console.WriteLine(error.ErrorMessage.ToString());
            }
            Console.WriteLine(ModelState.IsValid);
            Console.WriteLine("Didn't Pass");
            return View("WeddingForm");
        }
        db.Weddings.Add(newWedding);

        db.SaveChanges();

        return RedirectToAction("Weddings");
}

[HttpPost("weddings/{weddingId}/delete")]
public IActionResult Delete(int weddingId)
{
    Wedding? wedding = db.Weddings.FirstOrDefault(wedding => wedding.WeddingId == weddingId);
    db.Weddings.Remove(wedding);
    db.SaveChanges();
    return RedirectToAction("Weddings");
}


    [HttpGet("weddings/{weddingId}")]
    public IActionResult ViewWedding(int weddingId)
    {
    Wedding? wedding = db.Weddings.Include(wed => wed.RSVPs).ThenInclude(r => r.User).FirstOrDefault(wedding => wedding.WeddingId == weddingId);

    if(wedding == null)
    {
        return RedirectToAction("Index");
    }
    return View("Details", wedding);
}

[HttpPost("weddings/{weddingId}/RSVP")]
public IActionResult RSVPTo(int weddingId)
{
    RSVP? existingRSVP = db.RSVPs.FirstOrDefault(r=> r.UserId == HttpContext.Session.GetInt32("UUID")
    && r.WeddingId == weddingId);
    

    if(existingRSVP == null)
    {
    RSVP newRSVP = new RSVP()
    {
        WeddingId = weddingId,
        UserId = (int)HttpContext.Session.GetInt32("UUID")
    };

    db.RSVPs.Add(newRSVP);
    }
    else
    {
        db.RSVPs.Remove(existingRSVP);
    }
    db.SaveChanges();
    return RedirectToAction("Weddings");
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

