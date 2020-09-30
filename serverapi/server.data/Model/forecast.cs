using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace server.data.Model
{

  [Table("previsioni")]
  public class Forecast
  {
    public int Id { get; set; }

    public double TemperatureC { get; set; }

    public double Precipitation { get; set; }

    public DateTime Date { get; set; }

  }
}