<template>
  <div>
    <input type="text" v-model="Value" />
 
    <button class="search" @click="$emit('search', Value)"></button>
    <button class="remove" @click="Value = ''"></button>
  </div>
</template>
<script lang="ts" src="./inputCard.ts"></script>