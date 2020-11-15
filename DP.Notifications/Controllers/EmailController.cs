using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DP.Notifications.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DP.Notifications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailController : ControllerBase
    {
        private readonly EmailSenderService _sender;
        public EmailController(EmailSenderService sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public void SendMessage(Message msg)
        {
            _sender.SendMessage(msg.emailAddress, msg.subject, msg.body);
        }
    }

    public class Message
    {
        public string emailAddress { get; set; } 
        public string subject { get; set; }
        public string body { get; set; }
    }

}
