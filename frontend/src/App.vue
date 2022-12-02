<template>
  <div class="q-pa-md row q-gutter-md">
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="1"
            :margin="margin"
            :width="width * 0.48"
            :height="height * 0.47"
            :passengers-count="1000"
            :stations-count="4"
            :is-station-has-queue="false"
            :timeline="staticGeneralQueueTimelineCsv"
            :statistics="statistics.staticGeneralQueue"
        ></QueueChart>
      </q-card-section>
    </q-card>
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="2"
            :margin="margin"
            :width="width * 0.48"
            :height="height * 0.47"
            :passengers-count="1000"
            :stations-count="4"
            :is-station-has-queue="true"
            :timeline="staticShortestQueueTimelineCsv"
            :statistics="statistics.staticShortestQueue"
        ></QueueChart>
      </q-card-section>
    </q-card>
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="4"
            :margin="margin"
            :width="width * 0.48"
            :height="height * 0.47"
            :passengers-count="1000"
            :stations-count="5"
            :is-station-has-queue="false"
            :timeline="dynamicGeneralQueueTimelineCsv"
            :statistics="statistics.dynamicGeneralQueue"
        ></QueueChart>
      </q-card-section>
    </q-card>
    <q-card>
      <q-card-section style="padding: 0">
        <QueueChart
            id="3"
            :margin="margin"
            :width="width * 0.48"
            :height="height * 0.47"
            :passengers-count="1000"
            :stations-count="5"
            :is-station-has-queue="true"
            :timeline="dynamicShortestQueueTimelineCsv"
            :statistics="statistics.dynamicShortestQueue"
        ></QueueChart>
      </q-card-section>
    </q-card>
  </div>
</template>

<script>
import QueueChart from "@/components/QueueChart"
import {statistics} from "@/data/statistics"
import staticGeneralQueueTimelineCsv from '@/data/staticGeneralQueue.csv'
import staticShortestQueueTimelineCsv from '@/data/staticShortestQueue.csv'
import dynamicGeneralQueueTimelineCsv from '@/data/dynamicGeneralQueue.csv'
import dynamicShortestQueueTimelineCsv from '@/data/dynamicShortestQueue.csv'

export default {
  name: 'App',
  components: {
    QueueChart
  },
  data() {
    return {
      width: window.innerWidth,
      height: window.innerHeight,
      margin: {top: 0, right: 0, bottom: 0, left: 0},
      staticStationsCount: 4,
      passengersCount: 200,
      arrivalInterval: 2,
      takeInterval: 16,
      statistics,
      staticGeneralQueueTimelineCsv,
      staticShortestQueueTimelineCsv,
      dynamicGeneralQueueTimelineCsv,
      dynamicShortestQueueTimelineCsv,
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
