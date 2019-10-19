using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Models;

namespace Pathfinder.Controllers {
    public class EditorController : Controller {
        [Authorize]
        public IActionResult Index(RoomViewModel room) {
            room.VisitorsCount++;
            return View(room);
        }
    }
}