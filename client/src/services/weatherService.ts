import Axios from 'axios';

export class WeatherService {

  public static async GetForecasts(): Promise<data.Forecast[]> {
    let result = await Axios.get<data.Forecast[]>("https://localhost:5001/api/weather/forecasts");
    if (result.status == 200) return result.data;
    return [];
  }

  public static async Send(model: data.Forecast):
    Promise<data.Forecast | null> {
    model.date = new Date();
    let result = await Axios.post<data.Forecast>("https://localhost:5001/api/weather/forecasts", model);
    if (result.status == 200) return result.data;
    return null;
  }
}