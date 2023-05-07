using GameServer.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameServer.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TimeController : ControllerBase
{
    private readonly GameDbContext _context;

    public TimeController(GameDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public void WriteTime(TimePassed time)
    {
        _context.Add(time);
        _context.SaveChanges();
    }
}