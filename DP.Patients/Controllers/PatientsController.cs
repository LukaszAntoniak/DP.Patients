using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DP.Patients.Models;
using DP.Patients.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DP.Patients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly DpDataContext _context;
        private readonly ServiceBusSender _sender;

        public PatientsController(DpDataContext context, ServiceBusSender sender)
        {
            _context = context;
            _sender = sender;
        }

        [HttpGet]
        [Route("all")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(_context.Patients.ToList());
        }

        [HttpGet]
        [Route("email/{email}/send")]
        public async Task<IActionResult> GetSenderTest(string email)
        {
            await _sender.SendMessage(new MessagePayload() { EventName = "NewUserRegistered", UserEmail = email });
            return Ok();
        }

        [HttpPost]
        [Route("new")]
        [Authorize]
        public async Task<IActionResult> PostPatient(Patient newPatient)
        {
            _context.Patients.Add(newPatient);
            _context.SaveChanges();

            await _sender.SendMessage(new MessagePayload() { EventName = "NewUserRegistered", UserEmail = newPatient.Email });
            int id = newPatient.Id;
            return Created("/api/patients/", id);
        }

        [HttpPut]
        public IActionResult InvalidAction()
        {
            throw new InvalidOperationException("Testowy wyjątek"); 
        }
    }
}
