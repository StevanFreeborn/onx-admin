<script setup lang="ts">
  import { RouterView } from 'vue-router';
</script>

<template>
  <main>
    <header>
      <h1>OnxAdmin</h1>
    </header>
    <div class="main-content">
      <RouterView />
    </div>
    <footer>
      <p>OnxAdmin can make mistakes. Check important info</p>
    </footer>
  </main>
</template>

<style scoped>
  main {
    display: flex;
    justify-content: center;
    flex: 1;
    height: 0;

    & footer {
      text-align: center;

      & p {
        font-size: 0.75rem;
      }
    }
  }
</style>
