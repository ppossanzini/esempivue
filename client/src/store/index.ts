import Vue from 'vue'
import Vuex, { Store } from 'vuex'

Vue.use(Vuex)

interface IAlarm {
  id: number,
  text: string,
  resolved: boolean
}

export interface IStoreState {
  alarms: IAlarm[],
  forecast: data.Forecast[]
}

interface IStoreActions {
  setForecasts(context: any, model: data.Forecast[]): void;
}

const store = {
  state: {
    alarms: [],
    forecast: []
  } as IStoreState,
  getters: {
    getUserAlarms(state: IStoreState) {
      return state.alarms
    }
  },
  mutations: {
    setForecasts(state: IStoreState, model: data.Forecast[]) {
      state.forecast.push(...model);
    },
    refreshAllarmi(state: IStoreState) {
      for (const a of state.alarms) {
        a.resolved = true;
      }
    },
    addAlarm(state: IStoreState, model: { alarm: IAlarm, test: boolean }) {
      state.alarms.push(model.alarm);
    }
  },
  actions: {
    setForecasts(context: any, model: data.Forecast[]) {
      context.commit('setForecasts', model);
    },
    refreshAllarmi(context: any) {
      context.commit('refreshAllarmi');
    },
    addAlarm(context: any, model: { alarm: IAlarm, test: boolean }) {
      context.commit('addAlarm', model);
    }
  },
}

export const storeActions = store.actions as IStoreActions;

export default new Vuex.Store(store)
