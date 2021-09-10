const lodash = require("lodash");
const antd = require("antd");
const {bundleMDX} = require("mdx-bundler");
const prettier = require("prettier");

module.exports = async (arg, files, format) => {
  try {
    const mdxSource = arg.trim();
    const reducedFiles = files.reduce(
      (prev, current) => ({...prev, [current.path]: current.content}),
      {}
    );
    const result = await bundleMDX(mdxSource, {files: reducedFiles});
    const {code, frontmatter} = result;
    const formatted = prettier.format(arg, {semi: false, parser: "mdx"});
    return {code, frontmatter, formatted};
  } catch (E) {
    return {code: "", frontmatter: {}, formatted: ""}
  }
  // callback(null, code);
};
