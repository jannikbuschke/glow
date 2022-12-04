const react = require("@vitejs/plugin-react");

module.exports = {
  stories: ["../src/**/*.stories.mdx", "../src/**/*.stories.@(js|jsx|ts|tsx)"],
  addons: [
    "@storybook/addon-links",
    "@storybook/addon-essentials",
    "@storybook/addon-interactions",
  ],
  framework: "@storybook/react",
  core: {
    builder: "@storybook/builder-webpack5",
  },
  typescript: {
    reactDocgen: "react-docgen",
  },
  // viteFinal: async (config) => {
  //   config.plugins = config.plugins.filter(
  //     (plugin) =>
  //       !(Array.isArray(plugin) && plugin[0]?.name.includes("vite:react"))
  //   );
  //   config.resolve.alias = {
  //     ...config.resolve.alias,
  //     "@emotion/css": path.resolve(
  //       path.join(__dirname, "../node_modules/@emotion/react")
  //     ),
  //   };

  //   console.log(config.plugins);
  //   return config;
  // },
  babel: async (options) => ({
    ...options,
    presets: [
      ["@babel/preset-env", { shippedProposals: true, loose: true }],
      [
        "@babel/preset-react",
        { runtime: "automatic", importSource: "@emotion/react" },
      ],
      ["@babel/preset-typescript"],
    ],
  }),
};
