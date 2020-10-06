<template>
  <div class="list-page">
    <header>
      <label>pioggia</label>
      <input type="number" v-model.number="inputdata.precipitation" />

      <label>temperatura</label>
      <input type="number" v-model.number="inputdata.temperatureC" />

      <button class="send" @click="sendData()">invia</button>
    </header>

    <input-card v-model="searchText" v-on:search="console.log($event)"></input-card>

    <forecast v-for="(f, idx) in forecasts" :key="idx" :model="f"></forecast>
  </div>
</template>
<script lang="ts" src="./List.ts"/>
<style lang="less">
@import url("./List.less");
</style>