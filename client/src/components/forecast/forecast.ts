
import Vue from 'vue';
import Component from 'vue-class-component';
import { Prop, Watch } from 'vue-property-decorator';

@Component
export default class Forecast extends Vue {

  @Prop({ default: null })
  model!: data.Forecast;



  mounted() {

  }





}


