import React from "react"
import logo from "./logo.svg"
import "./App.css"
<<<<<<< HEAD
import { Tabs } from "antd"
import { ApplicationLayout } from "glow-react/es/Layout/Layout"
import "antd/dist/antd.css"
import { FilesExample } from "./files-example"
import { BrowserRouter as Router, Link } from "react-router-dom"

function App() {
  return (
    <Router>
      <ApplicationLayout Header={null}>
        <Link to="/portfolios/">Portfolios</Link>
        <Tabs style={{ margin: 100 }}>
          <Tabs.TabPane tab="Portfolios">
            <FilesExample />
          </Tabs.TabPane>
        </Tabs>
      </ApplicationLayout>
    </Router>
=======
import { Formik } from "formik"
import { Input, SubmitButton, Form } from "formik-antd"
import { notification } from "antd"
import { AntDesignOutlined } from "@ant-design/icons"
import { ApplicationLayout } from "glow-react/es/Layout/Layout"
import "antd/dist/antd.css"

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
      </header>

      <div style={{ background: "white", padding: "20px" }}>
        <Formik
          initialValues={{ field1: "hello world" }}
          onSubmit={(values, f) => {
            notification.info({ message: values.field1 })
            f.setSubmitting(false)
          }}
        >
          <Form>
            <Input name="field1" />
            <SubmitButton
              style={{ marginTop: 10 }}
              icon={<AntDesignOutlined />}
            >
              Submit
            </SubmitButton>
          </Form>
        </Formik>
      </div>
      <ApplicationLayout Header={<div>HEADER</div>}>
        <div>chilcren</div>
      </ApplicationLayout>
    </div>
>>>>>>> master
  )
}

export default App
