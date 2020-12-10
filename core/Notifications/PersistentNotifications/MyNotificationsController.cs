using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Glow.NotificationsCore
{

    [ApiController]
    [Route("api/my-notifications")]
    public class MyNotificationsController : ControllerBase
    {
        private readonly INotificationsService svc;

        public MyNotificationsController(INotificationsService svc)
        {
            this.svc = svc;
        }

        [HttpPost("mark-as-read")]
        public async Task<ActionResult> MarkAsRead(MarkAsRead request)
        {
            await svc.MarkAsRead(request.NotificationId.Value);
            return Ok();
        }

        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            return await svc.GetUnreadNotifications();
        }
    }
}
