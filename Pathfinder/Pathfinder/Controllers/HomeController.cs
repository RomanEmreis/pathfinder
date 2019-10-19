using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pathfinder.Application.Services;
using Pathfinder.Models;

namespace Pathfinder.Controllers {
    public class HomeController : Controller {
        private readonly IRoomService _roomService;

        public HomeController(IRoomService roomService) {
            _roomService = roomService;
        }

        [Authorize]
        public async Task<IActionResult> Index() {
            var activeRooms = await _roomService.GetCurrentRooms();
            return View(activeRooms);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRoom([RegularExpression(@"^\s*\S+\s*$")] string roomName) {
            if (!ModelState.IsValid) return View();

            var newRoom = new RoomViewModel { Name = roomName };
            await _roomService.Add(newRoom);

            return RedirectToAction(
               actionName: "Index",
               controllerName: "Editor",
               routeValues: newRoom);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error() {
            var viewModel = await GetCurrentErrorViewModelAsync();
            return View(viewModel);
        }

        private ValueTask<ErrorViewModel> GetCurrentErrorViewModelAsync() =>
            new ValueTask<ErrorViewModel>(
                new ErrorViewModel { 
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
    }
}