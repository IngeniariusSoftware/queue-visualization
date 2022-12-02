<template>
  <div :id="containerID.replace('#', '')"/>
</template>

<script>
import * as d3 from 'd3'

const componentName = 'QueueChart'
const timeScale = 1.0
const second = 1.0
const tolerance = 0.0001
const millisecondsInSecond = 1000.0 / timeScale
const stationsColors = ['#f7cc05', '#eb2d23', '#1c3f91', '#079bd7', '#00883a', '#f0859d', '#ae5e3b']

export default {
  name: componentName,
  data() {
    return {
      rootGroup: Selection,
      stationsGroup: Selection,
      passengersGroup: Selection,
      statisticsGroup: Selection,
      stations: [],
      passengers: [],
      generalPassengersQueue: [],
      timelineIndex: 0,
      lastArrivalTime: 0.0,
      statisticsTimerId: 0,
      time: -second,
    }
  },
  props: {
    id: {type: String, required: true},
    margin: {type: Object, required: true},
    width: {type: Number, required: true},
    height: {type: Number, required: true},
    timeline: {type: Array, required: true},
    passengersCount: {type: Number, required: true},
    stationsCount: {type: Number, required: true},
    isStationHasQueue: {type: Boolean, required: true},
    statistics: {type: Object, required: true},
  },
  computed: {
    containerID() {
      return `#${componentName}Container${this.id}`
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
    this.statisticsGroup = this.rootGroup.append('g')
    this.generatePassengers(this.passengersCount, this.passengers)
    this.generateStations(this.stationsGroup, this.stations, this.stationsCount)
    this.getPositionInGeneralQueue = this.isStationHasQueue ? this.getPositionWithoutGeneralQueue : this.getPositionWithGeneralQueue
    this.addStatistics()
    this.handleTimeline()
    setInterval(this.handleTimeline, millisecondsInSecond)
  },
  methods: {
    addStatistics() {
      const statisticsSelection = this.statisticsGroup.selectAll('text').data(this.statistics)
      statisticsSelection.enter()
          .append('text')
          .attr('x', 10)
          .attr('y', (_, i) => (i + 1) * 20.0)
          .merge(statisticsSelection)
          .text((d) => `${d.label}: ${JSON.stringify(d.value)}`)
      statisticsSelection.exit().remove()
    },

    handleTimeline() {
      this.time += second
      while (Math.abs(this.timeline[this.timelineIndex].time - this.time) < 1.0 + tolerance) {
        const event = this.timeline[this.timelineIndex]
        if (event['actor'] === 'T') {
          if (event['durationOrDestinationId'] === -1) {
            const nextEvent = this.timeline[this.timelineIndex + 1]
            let destination = -1
            if (nextEvent['actor'] === 'T' && event['id'] === nextEvent['id']) {
              destination = nextEvent['durationOrDestinationId']
              this.timelineIndex++
            }
            this.arrivePassenger(event, destination)
          } else {
            setTimeout(() => this.moveToStation(event, event['durationOrDestinationId']), millisecondsInSecond * 0.5)
          }
        } else {
          setTimeout(() => this.takePassenger(event), millisecondsInSecond)
        }

        this.timelineIndex++
      }
    },

    generatePassengers(passengersCount, passengers) {
      for (let i = 0; i < passengersCount; i++) {
        const graphicObject = this.passengersGroup.append('rect')
            .attr('x', -this.passengerGraphicModelSize)
            .attr('y', (this.innerHeight - this.passengerGraphicModelSize) * 0.5)
            .attr('width', this.passengerGraphicModelSize)
            .attr('height', this.passengerGraphicModelSize)
        passengers.push(graphicObject)
      }

      return passengers
    },

    generateStations(group, stations, stationsCount) {
      const heightBias = this.innerHeight / (stationsCount + 1)
      const lineWidth = this.passengerGraphicModelSize * 2.0

      for (let i = 0; i < stationsCount; i++) {
        const stationLine = group.append('rect')
            .attr('x', this.innerWidth - lineWidth)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.25))
            .attr('width', lineWidth + this.passengerGraphicModelSize)
            .attr('height', this.passengerGraphicModelSize * 0.5)
            .attr('fill', '#808080')
            .classed('stationLine', true)

        const platform = group.append('rect')
            .attr('x', this.innerWidth - lineWidth)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.5))
            .attr('width', this.passengerGraphicModelSize * 0.3)
            .attr('height', this.passengerGraphicModelSize)
            .attr('fill', '#808080')
            .classed('stationLine', true)

        const train = group.append('rect')
            .attr('x', this.innerWidth)
            .attr('y', (heightBias * (1 + i)) - (this.passengerGraphicModelSize * 0.5))
            .attr('width', this.passengerGraphicModelSize)
            .attr('height', this.passengerGraphicModelSize)
            .classed('train', true)

        stations.push({stationLine, platform, train, passengersQueue: []})
      }
    },

    arrivePassenger(passenger, destinationId) {
      const graphicModel = this.passengers[passenger.id]
      graphicModel.transition()
          .duration(millisecondsInSecond * (destinationId === -1 ? 1.0 : 0.5))
          .attr('x', this.getPositionInGeneralQueue(this.generalPassengersQueue.length))
          .on('end', () => {
            this.generalPassengersQueue.push(graphicModel)
            graphicModel.transition()
                .duration(millisecondsInSecond)
                .attr('x', this.getPositionInGeneralQueue(this.generalPassengersQueue.length - 1))
            if (destinationId !== -1) {
              this.moveToStation(passenger, destinationId)
            }
          })
    },

    moveToStation(passenger, stationId) {
      const graphicModel = this.passengers[passenger.id]
      const station = this.stations[stationId]
      station.passengersQueue.push(graphicModel)
      this.generalPassengersQueue.shift()
      this.orderGeneralQueue()
      graphicModel.attr('fill', stationsColors[stationId % stationsColors.length])
          .transition()
          .duration(millisecondsInSecond * 0.5)
          .attr('x', this.getPositionInStationQueue(station, station.passengersQueue.length - 1))
          .attr('y', station.platform.attr('y'))
          .on('end', () => {
            graphicModel.transition()
                .duration(millisecondsInSecond * 0.25)
                .attr('x', this.getPositionInStationQueue(station, station.passengersQueue.indexOf(graphicModel)))
          })
    },

    getPositionWithGeneralQueue(number) {
      return (this.innerWidth * 0.5) + ((1 - number) * 1.5 * this.passengerGraphicModelSize)
    },

    getPositionWithoutGeneralQueue() {
      return 1.5 * this.passengerGraphicModelSize
    },

    getPositionInStationQueue(station, number) {
      return station.platform.attr('x') - this.passengerGraphicModelSize - (1.5 * this.passengerGraphicModelSize * number)
    },

    orderGeneralQueue() {
      this.generalPassengersQueue.forEach((x, i) => x.transition()
          .duration(millisecondsInSecond * 0.5)
          .attr('x', this.getPositionInGeneralQueue(i))
      )
    },

    takePassenger(stationEvent) {
      const station = this.stations[stationEvent['id']]
      station.platform.attr('fill', stationsColors[stationEvent['id'] % stationsColors.length])
      station.stationLine.attr('fill', stationsColors[stationEvent['id'] % stationsColors.length])
      station.train.transition()
          .duration(stationEvent['durationOrDestinationId'] * 0.5 * millisecondsInSecond)
          .attr('x', station.platform.attr('x') - this.passengerGraphicModelSize)
          .on('end', () => {
            const passengerGraphicModel = station.passengersQueue[0]
            passengerGraphicModel.transition()
                .duration(stationEvent['durationOrDestinationId'] * 0.5 * millisecondsInSecond)
                .attr('x', this.innerWidth)

            setTimeout(() => {
              station.passengersQueue.shift()
              this.orderStationQueue(station)
            }, stationEvent['durationOrDestinationId'] * 0.25 * millisecondsInSecond)

            station.train.transition()
                .duration(stationEvent['durationOrDestinationId'] * 0.5 * millisecondsInSecond)
                .attr('x', this.innerWidth)
                .on('end', () => {
                  if (station.passengersQueue.length === 0) {
                    station.stationLine.attr('fill', '#808080')
                    station.platform.attr('fill', '#808080')
                  }
                })
          })
    },

    orderStationQueue(station) {
      station.passengersQueue.forEach((x, i) => x.transition()
          .duration(millisecondsInSecond * 0.5)
          .attr('x', this.getPositionInStationQueue(station, i))
          .attr('y', station.platform.attr('y'))
      )
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
</script>

<style>
.train {
  stroke-width: 4px;
  stroke: black;
  fill: none;
}
</style>