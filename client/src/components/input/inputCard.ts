import Vue from 'vue'
import Component from 'vue-class-component';
import { Prop } from 'vue-property-decorator';

@Component
export default class InputCard extends Vue {

  @Prop()
  value!: string;

  get Value() { return this.value; }
  set Value(v: string) { //NO => this.value = v; 
    this.$emit('input', v);
  }
}