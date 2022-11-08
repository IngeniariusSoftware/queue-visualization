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
      getPositionInGeneralQueue: undefined,
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
    takeInterval: {type: Number, required: true},
    isStationHasQueue: {type: Boolean, required: true},
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
    this.generatePassengers(this.passengersCount, this.passengers, this.arrivalInterval)
    this.generateStations(this.stationsGroup, this.stations, this.stationsCount)
    this.getPositionInGeneralQueue = this.isStationHasQueue ? this.getPositionWithoutGeneralQueue : this.getPositionWithGeneralQueue
    this.arrivePassenger()
  },
  methods: {
    generatePassengers(passengersCount, passengers, arrivalInterval) {
      let arrivalTime = 0
      for (let i = 0; i < passengersCount; i++) {
        arrivalTime += generateInterval(arrivalInterval)
        const graphicObject = this.passengersGroup.append('rect')
            .attr('x', -this.passengerGraphicModelSize)
            .attr('y', (this.innerHeight - this.passengerGraphicModelSize) * 0.5)
            .attr('width', this.passengerGraphicModelSize)
            .attr('height', this.passengerGraphicModelSize)
        passengers.push({number: i, arrivalTime, graphicObject})
      }

      return passengers
    },
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

        const platform = group.append('rect')
            .attr('x', this.innerWidth - lineWidth)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.5))
            .attr('width', this.passengerGraphicModelSize * 0.3)
            .attr('height', this.passengerGraphicModelSize)
            .attr('fill', stationsColors[i % stationsColors.length])
            .classed('stationLine', true)

        const train = group.append('rect')
            .attr('x', this.innerWidth)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.5))
            .attr('width', this.passengerGraphicModelSize)
            .attr('height', this.passengerGraphicModelSize)
            .classed('train', true)

        stations.push({number: i, platform, train, passengersQueue: []})
      }
    },
    arrivePassenger() {
      if (this.arrivalPassengerIndex >= this.passengers.length) {
        return
      }

      const passenger = this.passengers[this.arrivalPassengerIndex]
      const timeout = (passenger.arrivalTime - this.lastArrivalTime - second) * millisecondsInSecond
      this.lastArrivalTime = passenger.arrivalTime
      setTimeout(() => passenger.graphicObject.transition()
              .duration(millisecondsInSecond)
              .attr('x', this.getPositionInGeneralQueue(this.generalPassengersQueue.length))
              .on('end', () => {
                this.generalPassengersQueue.push(this.passengers[this.arrivalPassengerIndex])
                this.arrivalPassengerIndex++
                passenger.graphicObject.transition()
                    .duration(millisecondsInSecond * 0.5)
                    .attr('x', this.getPositionInGeneralQueue(this.generalPassengersQueue.length - 1))
                this.onArrivedPassenger()
              }),
          timeout
      )
    },
    getPositionWithGeneralQueue(number) {
      return (this.innerWidth * 0.5) + ((1 - number) * 1.5 * this.passengerGraphicModelSize)
    },
    getPositionWithoutGeneralQueue() {
      return 1.5 * this.passengerGraphicModelSize
    },
    onArrivedPassenger() {
      this.trySelectStation()
      this.arrivePassenger()
    },
    trySelectStation() {
      if (this.generalPassengersQueue.length === 0) {
        return
      }

      const freeStation = this.getFreeStation()
      if (freeStation === undefined) {
        return
      }

      const passenger = this.generalPassengersQueue.shift()
      this.orderGeneralQueue()

      freeStation.passengersQueue.push(passenger)
      passenger.graphicObject.attr('fill', freeStation.platform.attr('fill'))
          .transition()
          .duration(millisecondsInSecond * 0.5)
          .attr('x', freeStation.platform.attr('x') - this.passengerGraphicModelSize)
          .attr('y', freeStation.platform.attr('y'))

      this.takePassenger(passenger, freeStation)
      this.trySelectStation()
    },
    getFreeStation() {
      let freeStation = undefined
      if (!this.isStationHasQueue) {
        const freeStations = this.stations.filter(x => x.passengersQueue.length === 0)
        if (freeStations.length > 0) {
          freeStation = freeStations[Math.floor(Math.random() * freeStations.length)]
        }
      }

      return freeStation
    },
    orderGeneralQueue() {
      this.generalPassengersQueue.forEach((x, i) => x.graphicObject.transition()
          .duration(millisecondsInSecond * 0.5)
          .attr('x', this.getPositionInGeneralQueue(i))
      )
    },
    takePassenger(passenger, station) {
      const duration = generateInterval(this.takeInterval) * millisecondsInSecond * 0.5
      station.train.transition()
          .duration(duration)
          .attr('x', station.platform.attr('x') - this.passengerGraphicModelSize)
          .on('end', () => {
            passenger.graphicObject.transition()
                .duration(duration)
                .attr('x', this.innerWidth)

            station.train.transition()
                .duration(duration)
                .attr('x', this.innerWidth)
                .on('end', () => {
                  station.passengersQueue.shift()
                  this.trySelectStation()
                })
          })
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

function generateInterval(interval) {
  return Math.floor(second + (Math.random() * interval))
}
</script>

<style>
.train {
  stroke-width: 4px;
  stroke: black;
  fill: none;
}

.stationLine {
  stroke-width: 0.5px;
  stroke: black;
}
</style>