import * as React from "react"
import { Formik } from "formik"
import { Form, Input, SubmitButton } from "formik-antd"
import * as colors from "@ant-design/colors"
import { presetPalettes } from "@ant-design/colors"
import { Button, message } from "antd"

export default {
  title: "Controller",
}

export const text = () => (
  <div style={{ padding: 20, background: colors.grey[2] }}>Hello World</div>
)
