<template>
  <div class="q-pa-md row q-gutter-md">
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="1"
            :margin="{top: 0, right: 0, bottom: 0, left: 0}"
            :width="width * 0.48"
            :height="height * 0.47"
            :arrival-interval="1"
            :take-interval="15"
            :passengers-count="30"
            :stations-count="5"
            :is-station-has-queue="false"
        ></QueueChart>
      </q-card-section>
    </q-card>
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="2"
            :margin="{top: 0, right: 0, bottom: 0, left: 0}"
            :width="width * 0.48"
            :height="height * 0.47"
            :arrival-interval="1"
            :take-interval="15"
            :passengers-count="30"
            :stations-count="5"
            :is-station-has-queue="true"
        ></QueueChart>
      </q-card-section>
    </q-card>
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="3"
            :margin="{top: 0, right: 0, bottom: 0, left: 0}"
            :width="width * 0.48"
            :height="height * 0.47"
            :arrival-interval="1"
            :take-interval="15"
            :passengers-count="30"
            :stations-count="5"
            :is-station-has-queue="false"
        ></QueueChart>
      </q-card-section>
    </q-card>
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="4"
            :margin="{top: 0, right: 0, bottom: 0, left: 0}"
            :width="width * 0.48"
            :height="height * 0.47"
            :arrival-interval="1"
            :take-interval="15"
            :passengers-count="30"
            :stations-count="5"
            :is-station-has-queue="true"
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
