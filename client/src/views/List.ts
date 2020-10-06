import Vue from 'vue';
import Component from 'vue-class-component';
import forecast from "@/components/forecast/forecast.vue";
import { WeatherService } from '@/services/weatherService';
import { IStoreState } from '@/store';



@Component({ components: { forecast } })
export default class List extends Vue {

  get forecasts(): data.Forecast[] {
    return (this.$store.state as IStoreState).forecast;
  }

  searchText: string = "";

  inputdata: data.Forecast = {
    temperatureC: 0,
    precipitation: 0
  };

  async mounted() {
    await WeatherService.GetForecasts(this.$store);
  }

  sendData() {

    WeatherService.Send(this.$store, this.inputdata);

    this.inputdata = {
      temperatureC: 0,
      precipitation: 0
    }

  }
}