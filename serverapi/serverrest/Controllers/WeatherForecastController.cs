using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using server.data.Model;

namespace serverrest.Controllers
{
  [ApiController]
  [Route("api/weather")]
  public class WeatherForecastController : ControllerBase
  {
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly DB _db;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, DB db)
    {
      _logger = logger;
      _db = db;
    }

    [HttpGet("forecasts")]
    public async Task<IEnumerable<Forecast>> GetAll()
    {
      return await this._db.Forecasts.ToListAsync();
    }

    [HttpGet("forecasts/{id}")]
    public async Task<Forecast> Get(int id)
    {
      return await this._db.Forecasts.Where(f => f.Id == id).FirstOrDefaultAsync();
    }

    [HttpDelete("forecasts/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      var forecast = await _db.Forecasts.Where(f => f.Id == id).FirstOrDefaultAsync();
      if (forecast == null) return NotFound();
      _db.Forecasts.Remove(forecast);
      await _db.SaveChangesAsync();
      return Ok();
    }

    [ProducesResponseType(typeof(Forecast), 200)]
    [HttpPost, HttpPut, Route("forecasts")]
    public async Task<IActionResult> Add([FromBody] Forecast model)
    {
      _db.Forecasts.Add(model);
      await _db.SaveChangesAsync();
      return Ok(model);
    }

  }
}
