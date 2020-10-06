import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

interface IAlarm {
  id: number,
  text: string,
  resolved: boolean
}

export default new Vuex.Store({
  state: {
    alarms: []
  },
  getters: {
    getUserAlarms(state) {
      return state.alarms
    }
  },
  mutations: {
    refreshAllarmi(state) {
      for (const a of state.alarms as IAlarm[]) {
        a.resolved = true;
      }
    },
    addAlarm(state, model: { alarm: IAlarm, test: boolean }) {
      (state.alarms as IAlarm[]).push(model.alarm);
    }
  },
  actions: {
    refreshAllarmi(context) {
      context.commit('refreshAllarmi');
    },
    addAlarm(context, model: { alarm: IAlarm, test: boolean }) {
      context.commit('addAlarm', model);
    }
  },

})
