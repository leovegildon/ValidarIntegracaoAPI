using Microsoft.AspNetCore.Mvc;
using ValidarIntegracaoAPI.Models;

namespace ValidarIntegracaoAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EventosFiscaisController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<EventosFiscaisController> _logger;

        public EventosFiscaisController(ILogger<EventosFiscaisController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetEventosFiscais")]
        public IEnumerable<EventosFiscais> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new EventosFiscais
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}