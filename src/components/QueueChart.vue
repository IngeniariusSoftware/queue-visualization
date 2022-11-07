<template>
  <div :id="containerID.replace('#', '')"/>
</template>


<script>
import * as d3 from 'd3'

const componentName = 'QueueChart'
const timeScale = 1.0
const second = 1.0 / timeScale
const millisecondsInSecond = 1000.0 * second
const stationsColors = ['#f7cc05', '#eb2d23', '#1c3f91', '#079bd7', '#00883a', '#f0859d', '#ae5e3b']

export default {
  name: componentName,
  data() {
    return {
      rootGroup: Selection,
      stationsGroup: Selection,
      passengersGroup: Selection,
      stations: [],
      passengers: [],
      generalPassengersQueue: [],
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
    stationsCount: {type: Number, required: true},
    passengersCount: {type: Number, required: true},
    arrivalInterval: {type: Number, required: true},
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
    passengerGraphicModelSize() {
      return this.innerHeight / ((this.stationsCount + 1.0) * 2.0)
    },
  },
  mounted() {
    this.rootGroup = addRootGroup(this.containerID, this.width, this.height, this.margin)
    this.stationsGroup = this.rootGroup.append('g')
    this.passengersGroup = this.rootGroup.append('g')
    this.generateStations(this.stationsGroup, this.stations, this.stationsCount)
    generatePassengers(this.passengersCount, this.passengers, this.arrivalInterval)
    this.arrivePassenger()
  },
  methods: {
    generateStations(group, stations, stationsCount) {
      const heightBias = this.innerHeight / (stationsCount + 1)
      const lineWidth = this.passengerGraphicModelSize * 2.0

      for (let i = 0; i < stationsCount; i++) {
        group.append('rect')
            .attr('x', this.innerWidth - lineWidth)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.25))
            .attr('width', lineWidth + this.passengerGraphicModelSize)
            .attr('height', this.passengerGraphicModelSize * 0.5)
            .attr('fill', stationsColors[i % stationsColors.length])
            .classed('stationLine', true)

        const station = group.append('rect')
            .attr('x', this.innerWidth - lineWidth)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.5))
            .attr('width', this.passengerGraphicModelSize * 0.3)
            .attr('height', this.passengerGraphicModelSize)
            .attr('fill', stationsColors[i % stationsColors.length])
            .classed('stationLine', true)

        const train = group.append('rect')
            .attr('x', this.innerWidth + this.passengerGraphicModelSize)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.5))
            .attr('width', this.passengerGraphicModelSize)
            .attr('height', this.passengerGraphicModelSize)
            .classed('train', true)

        stations.push({number: i, station, train})
      }
    },
    arrivePassenger() {
      if (this.arrivalPassengerIndex >= this.passengers.length)
        return

      const passenger = this.passengers[this.arrivalPassengerIndex]
      passenger.graphicObject = this.passengersGroup.append('rect')
          .attr('x', -this.passengerGraphicModelSize)
          .attr('y', (this.innerHeight - this.passengerGraphicModelSize) * 0.5)
          .attr('width', this.passengerGraphicModelSize)
          .attr('height', this.passengerGraphicModelSize)

      const timeout = (passenger.arrivalTime - this.lastArrivalTime - second) * millisecondsInSecond
      this.lastArrivalTime = passenger.arrivalTime
      this.arrivalPassengerIndex++
      this.generalPassengersQueue.push(this.passengers[this.arrivalPassengerIndex])
      setTimeout(() => passenger.graphicObject.transition()
              .duration(this.passengerArrivalAnimationDuration)
              .attr('x', this.getPositionInGeneralQueue())
              .on('end', this.onArrivedPassenger),
          timeout
      )
    },
    getPositionInGeneralQueue() {
      return (this.innerWidth * 0.5) + ((2 - this.generalPassengersQueue.length) * 1.5 * this.passengerGraphicModelSize)
    },
    onArrivedPassenger() {
      this.arrivePassenger()
    },
  }
}

function addRootGroup(containerID, width, height, margin) {
  return d3.select(containerID)
      .append('svg')
      .attr('width', width)
      .attr('height', height)
      .append('g')
      .attr('transform', `translate(${margin.left}, ${margin.top})`)
}

function generatePassengers(passengersCount, passengers, arrivalInterval) {
  let arrivalTime = 0
  for (let i = 0; i < passengersCount; i++) {
    arrivalTime += generateArrivalInterval(arrivalInterval)
    passengers.push({number: i, arrivalTime: arrivalTime})
  }

  return passengers
}

function generateArrivalInterval(arrivalInterval) {
  return Math.floor(second + (Math.random() * arrivalInterval))
}
</script>

<style>
.train {
  stroke-width: 2px;
  stroke: black;
  fill: none;
}

.stationLine {
  stroke-width: 0.5px;
  stroke: black;
}
</style>