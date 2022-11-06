<template>
  <div :id="containerID.replace('#', '')"/>
</template>


<script>
import * as d3 from 'd3'

const componentName = 'QueueChart'
const millisecondsInSecond = 1000
const stationsColors = ['#f7cc05', '#eb2d23', '#1c3f91', '#079bd7', '#00883a', '#f0859d', '#ae5e3b']

export default {
  name: componentName,
  data() {
    return {
      rootGroup: Selection,
      stations: [],
      passengers: generatePassengers(10, 5),
      arrivalPassengerIndex: 0,
      lastArrivalTime: 0.0,
      passengerArrivalAnimationDuration: millisecondsInSecond,
    }
  },
  props: {
    id: {type: String, required: true},
    margin: {type: Object, required: true},
    width: {type: Number, required: true},
    height: {type: Number, required: true},
  },
  computed: {
    containerID() {
      return `#${componentName}Container${this.id}`
    },
    visibleAreaID() {
      return `#${componentName}VisibleDefs${this.id}`
    },
    innerWidth() {
      return this.width - this.margin.left - this.margin.right
    },
    innerHeight() {
      return this.height - this.margin.top - this.margin.bottom
    },
  },
  mounted() {
    this.rootGroup = addRootGroup(this.containerID, this.width, this.height, this.margin)
    generateStations(this.rootGroup, this.stations, 5)
    this.arrivePassenger()
  },
  methods: {
    arrivePassenger() {
      if (this.arrivalPassengerIndex >= this.passengers.length)
        return

      const passenger = this.passengers[this.arrivalPassengerIndex]
      passenger.graphicObject = this.rootGroup.append('rect')
          .attr('x', -20)
          .attr('y', this.innerHeight * 0.5)
          .attr('width', 20)
          .attr('height', 20)

      const timeout = (passenger.arrivalTime - this.lastArrivalTime - 1) * millisecondsInSecond
      this.arrivalPassengerIndex++
      setTimeout(() => passenger.graphicObject.transition()
              .duration(this.passengerArrivalAnimationDuration)
              .attr('x', this.innerWidth / (this.passengers.length + 2) * (this.passengers.length - passenger.number))
              .on('end', this.arrivePassenger),
          timeout
      )
    },
  }
}

function generateStations(group, stations, stationsCount) {
  for (let i = 0; i < stationsCount; i++) {
    const train = group.append('rect')
        .attr('x', 20)
        .attr('y', 50 * (i + 1))
        .attr('width', 20)
        .attr('height', 20)
        .classed('train', true)
    stations.push({number: i, trainGraphic: train})
  }
}

function generatePassengers(passengersCount, arrivalInterval) {
  let arrivalTime = 0
  const passengers = []
  for (let i = 0; i < passengersCount; i++) {
    arrivalTime += generateArrivalInterval(arrivalInterval)
    passengers.push({number: i, arrivalTime: arrivalTime})
  }

  return passengers
}

function generateArrivalInterval(arrivalInterval) {
  return Math.floor(1 + (Math.random() * arrivalInterval))
}

function addRootGroup(containerID, width, height, margin) {
  return d3.select(containerID)
      .append('svg')
      .attr('width', width)
      .attr('height', height)
      .append('g')
      .attr('transform', `translate(${margin.left}, ${margin.top})`)
}
</script>

<style>
.train {
  stroke-width: 2px;
  stroke: black;
  fill: none;
}
</style>