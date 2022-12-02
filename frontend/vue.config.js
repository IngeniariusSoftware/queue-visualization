const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: [
    'csv-loader',
    'quasar'
  ],

  chainWebpack: config => {
    config
        .module
        .rule('csv')
        .test(/\.csv$/)
        .use('csv-loader')
        .loader('csv-loader')
        .options({
          dynamicTyping: true,
          header: true,
          skipEmptyLines: true,
          transformHeader: true
        })
        .end()
  },

  pluginOptions: {
    quasar: {
      importStrategy: 'kebab',
      rtlSupport: false
    }
  }
})
