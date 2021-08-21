const lodash = require('lodash');
const {bundleMDX} = require('mdx-bundler')

module.exports = async (arg) => {
    const mdxSource = `
---
title: Example Post
published: 2021-02-13
description: This is some description
---

# Wahoo

import Demo from './demo'

Here's a **neat** demo:

<Demo />
`.trim()

const result = await bundleMDX(mdxSource, {
  files: {
    './demo.tsx': `
import * as React from 'react'

function Demo() {
  return <div>Neat demo!</div>
}

export default Demo
    `,
  },
})

const {code, frontmatter} = result
return code
// callback(null, code);
}


