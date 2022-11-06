<template>
  <div class="q-pa-md row q-gutter-md">
    <q-card>
      <q-card-section style="padding: 8px">
        <QueueChart
            id="1"
            :margin="{top: 0, right: 0, bottom: 0, left: 0}"
            :width="width * 0.97"
            :height="height * 0.9"
        ></QueueChart>
      </q-card-section>
    </q-card>
  </div>
</template>

<script>
import QueueChart from "@/components/QueueChart"

export default {
  name: 'App',
  components: {
    QueueChart
  },
  data() {
    return {
      width: window.innerWidth,
      height: window.innerHeight
    }
  },
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
</style>
