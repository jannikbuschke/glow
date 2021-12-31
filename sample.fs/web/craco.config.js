module.exports = {
  style: {
    // modules: {
    //     localIdentName: ""
    // },
    // css: {
    //     loaderOptions: { /* Any css-loader configuration options: https://github.com/webpack-contrib/css-loader. */ },
    //     loaderOptions: (cssLoaderOptions, { env, paths }) => { return cssLoaderOptions; }
    // },
    // sass: {
    //     loaderOptions: { /* Any sass-loader configuration options: https://github.com/webpack-contrib/sass-loader. */ },
    //     loaderOptions: (sassLoaderOptions, { env, paths }) => { return sassLoaderOptions; }
    // },

    postcss: {
      env: {
        stage: 3,
        autoprefixer: {
          flexbox: "no-2009",
        },
        features: {
          "nesting-rules": true,
        },
      },
      plugins: [
        // require("postcss-flexbugs-fixes"),
        // require("postcss-preset-env")({
        //   autoprefixer: {
        //     flexbox: "no-2009",
        //   },
        //   stage: 3,
        //   features: {
        //     "nesting-rules": true,
        //   },
        // }),
        // require("postcss"),
        // require("postcss-loader"),
        // require("postcss-nested"),
        require("postcss-simple-vars"),
      ],
      // mode: "extends" /* (default value) */ || "file",
      // plugins: [require('plugin-to-append')], // Additional plugins given in an array are appended to existing config.
      // plugins: (plugins) => [require('plugin-to-prepend')].concat(plugins), // Or you may use the function variant.
      // env: {
      //     autoprefixer: { /* Any autoprefixer options: https://github.com/postcss/autoprefixer#options */ },
      //     stage: 3, /* Any valid stages: https://cssdb.org/#staging-process. */
      //     features: { /* Any CSS features: https://preset-env.cssdb.org/features. */ }
      // },
      // loaderOptions: { /* Any postcss-loader configuration options: https://github.com/postcss/postcss-loader. */ },
      // loaderOptions: (postcssLoaderOptions, { env, paths }) => { return postcssLoaderOptions; }
    },
  },
}
