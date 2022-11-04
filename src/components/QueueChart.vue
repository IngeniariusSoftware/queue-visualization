<template>
  <div :id="containerID.replace('#', '')"/>
</template>


<script>
import * as d3 from 'd3'

const componentName = 'QueueChart'

export default {
  name: componentName,
  data() {
    return {
      rootGroup: Selection,
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
    }
  },
  mounted() {
    this.rootGroup = addRootGroup(this.containerID, this.width, this.height, this.margin)
  },
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

<style scoped>

</style>