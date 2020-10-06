import Vue from 'vue';
import Component from 'vue-class-component';
import forecast from "@/components/forecast/forecast.vue";
import { WeatherService } from '@/services/weatherService';


@Component({ components: { forecast} })
export default class List extends Vue {

  forecasts: data.Forecast[] = [];

  searchText: string = "";

  inputdata: data.Forecast = {
    temperatureC: 0,
    precipitation: 0
  };

  async mounted() {
    this.forecasts = await WeatherService.GetForecasts();
    
  }

  async sendData() {

    let result = await WeatherService.Send(this.inputdata);
    if (result)
      this.forecasts.push(result);

    this.inputdata = {
      temperatureC: 0,
      precipitation: 0
    }

  }
}