import { storeActions } from '@/store';
import Axios from 'axios';
import Vue from 'vue';

export class WeatherService {

  public static async GetForecasts(store: any): Promise<data.Forecast[]> {
    let result = await Axios.get<data.Forecast[]>("https://localhost:5001/api/weather/forecasts");
    if (result.status == 200) {
      storeActions.setForecasts(store, result.data);
      return result.data;
    }
    return [];
  }

  public static Send(store: any, model: data.Forecast): void {
    model.date = new Date();
    storeActions.setForecasts(store, [model]);
    Axios.post<data.Forecast>("https://localhost:5001/api/weather/forecasts", model)
      .then(result => {
        if (result.status == 200) {
          Object.assign(model, result.data);
        }
      }).catch(err => {
        Vue.set(model, 'hasError', true);
      })
  }
}