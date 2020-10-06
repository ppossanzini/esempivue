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

  get filteredforecasts(): data.Forecast[] {
    return this.forecasts.filter(f => !this.searchText ||
      this.searchFunction.bind(f)());
  }

  searchText: string = "";

  get searchFunction(): Function {
    try {
      return eval(`function t(){ return this.precipitation ${this.searchText};}t`) as Function;
    }
    catch {
      return () => false;
    }

  }

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