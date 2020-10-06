<template>
  <div class="list-page">
    <header>
      <label>pioggia</label>
      <input type="number" v-model.number="inputdata.precipitation" />

      <label>temperatura</label>
      <input type="number" v-model.number="inputdata.temperatureC" />

      <button class="send" @click="sendData()">invia</button>
    </header>

    <input-card
      v-model="searchText"
      v-on:search="console.log($event)"
    ></input-card>
    <span v-if="forecasts.length - filteredforecasts.length"
      >Ci sono altri {{ forecasts.length - filteredforecasts.length }} risultati
      esclusi dalla ricerca</span
    >
    <div ref="external"></div>
    <card>
    <forecast
      v-for="(f, idx) in filteredforecasts"
      :key="idx"
      :model="f"
    ></forecast>
    </card>
  </div>
</template>
<script lang="ts" src="./List.ts"/>
<style lang="less">
@import url("./List.less");
</style>