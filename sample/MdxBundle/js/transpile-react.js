const lodash = require('lodash');
const antd = require("antd")
const {bundleMDX} = require('mdx-bundler')

module.exports = async (arg, files) => {
  const mdxSource = arg.trim()
  const reducedFiles = files.reduce((prev, current) => ({...prev, [current.path]: current.content}), {})
  const result = await bundleMDX(mdxSource, {files: reducedFiles})
  const {code, frontmatter} = result
  return {code, frontmatter}
// callback(null, code);
}


