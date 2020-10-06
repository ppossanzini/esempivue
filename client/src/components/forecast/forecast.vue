<template>
  <div class="forecast">
    <small>{{model.id}}</small>
    <label>#</label>

    <span>{{model.precipitation}}</span>
    <label>pioggia</label>

    <span>{{model.temperatureC}}</span>
    <label>temperatura</label>

    <time>{{model.date}}</time>
    <label>data</label>
  </div>
</template>

<script lang="ts" src="./forecast.ts">
</script>
<style lang="less">
@import "./forecast.less";
</style>