<script lang="ts" setup>
  import { onMounted, ref } from 'vue';

  interface WeatherForecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
  }

  type Forecasts = WeatherForecast[];

  const forecasts = ref<Forecasts>([]);
  const loading = ref<boolean>(true);
  const error = ref<Error | null>(null);

  onMounted(async () => {
    try {
      const response = await fetch('api/weatherforecast');
      const data = await response.json();
      forecasts.value = data;
    } catch (err) {
      if (err instanceof Error) {
        error.value = err;
      }
    } finally {
      loading.value = false;
    }
  });
</script>

<template>
  <table>
    <thead>
      <tr>
        <th>Date</th>
        <th>Temp. (C)</th>
        <th>Temp. (F)</th>
        <th>Summary</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="[index, forecast] in forecasts.entries()" :key="index">
        <td>{{ forecast.date }}</td>
        <td>{{ forecast.temperatureC }}</td>
        <td>{{ forecast.temperatureF }}</td>
        <td>{{ forecast.summary }}</td>
      </tr>
    </tbody>
  </table>
</template>

<style>
  table {
    border: none;
    border-collapse: collapse;
  }

  th {
    font-size: x-large;
    font-weight: bold;
    border-bottom: solid 0.2rem hsla(160, 100%, 37%, 1);
  }

  th,
  td {
    padding: 1rem;
  }

  td {
    text-align: center;
    font-size: large;
  }

  tr:nth-child(even) {
    background-color: var(--vt-c-black-soft);
  }
</style>
