<script setup lang="ts">
  import { RouterView } from 'vue-router';
</script>

<template>
  <div class="layout-container">
    <aside></aside>
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

      .main-content {
        flex: 1;
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
