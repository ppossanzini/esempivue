import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'

Vue.config.productionTip = false

import * as components from "@/components/index";
import "@/services/hub";

for (const compname in components) {
  Vue.component(compname, (components as any)[compname]);
}


new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
