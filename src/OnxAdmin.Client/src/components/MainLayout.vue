<script setup lang="ts">
  import { RouterView } from 'vue-router';
</script>

<template>
  <div class="layout-container">
    <!-- TODO: Implement a sidebar -->
    <!-- <aside></aside> -->
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
  </div>
</template>

<style scoped>
  .layout-container {
    display: flex;
    height: 100%;
    flex: 1;
    gap: 1rem;

    aside {
      width: 200px;
      background-color: var(--secondary-background-color);
    }

    main {
      display: flex;
      flex-direction: column;
      flex: 1;
      padding: 1rem;
      max-width: 100%;

      .main-content {
        display: flex;
        align-items: center;
        flex-direction: column;
        flex: 1;
        height: 0;
      }

      & footer {
        text-align: center;

        & p {
          font-size: 0.75rem;
        }
      }
    }
  }
</style>
