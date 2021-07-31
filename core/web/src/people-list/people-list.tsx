import * as React from "react"
import styled from "styled-components"
import { Avatar, Button } from "antd"
import { Input } from "formik-antd"
// import { CSSTransition, TransitionGroup } from "react-transition-group"
import "./list.css"
import { Formik } from "formik"
// import useSWR from "swr"
import { Alert } from "antd"
import { UserName, Email } from "./auto-complete"
import { MinusOutlined, PlusOutlined } from "@ant-design/icons"

const Container = styled.div`
  display: flex;
  align-items: center;
`

const Details = styled.div`
  flex: 1;
  margin-left: 5px;
`

const NameInput = styled(UserName)`
  font-weight: bold;
  border: none;
  background: none;
  width: 100%;
  flex: 1;
`

const EmailInput = styled(Email)`
  border: none;
  background: none;
  color: #929292;
`

const Delete = styled(Button)<{ hover: boolean }>`
  border: none;
  opacity: ${(props) => (props.hover ? "1" : "0.1")};
`

export function Item({ path, hover }: { path: string; hover: boolean }) {
  return (
    <Container>
      <Avatar size="large" />
      <Details>
        <NameInput size="small" path={path} dataTestId="user" />
        {/* <NameInput size="small" name="name" /> */}
        <EmailInput
          size="small"
          path={path}
          dataTestId="email"
          disabled={false}
        />
        {/* <EmailInput size="small" name="email" /> */}
      </Details>
      <Delete
        danger={true}
        ghost={true}
        icon={<MinusOutlined />}
        hover={hover}
      />
    </Container>
  )
}

const Add = styled(Button)<{ hover: boolean }>`
  opacity: ${(props) => (props.hover ? "1" : "0.02")};
  margin-right: 8px;
`

export function PeopleList() {
  // const { data, error } = useSWR(
  //   "/api/tops/user-search?api-version=3.0&search=a",
  // )

  const [hover, setHover] = React.useState(false)
  const [items, setItems] = React.useState<{ active: boolean; name: string }[]>(
    [
      { active: true, name: "item 1" },
      { active: true, name: "item 2" },
    ],
  )
  React.useEffect(() => {
    if (items.filter((v) => !v.active).length) {
      setItems(items.map((item) => ({ ...item, active: true })))
    }
  }, [items])
  return (
    <Formik
      initialValues={{
        user: [{ name: "Jannik Buschke", email: "jbu@phat.de" }],
      }}
      onSubmit={() => {}}
    >
      <div
        onMouseEnter={() => setHover(true)}
        onMouseLeave={() => setHover(false)}
      >
        {/* <pre>data: {JSON.stringify({ data }, null, 4)}</pre> */}
        {/* {error && (
          <Alert
            banner={true}
            showIcon={false}
            message={error.toString()}
            type="error"
          />
        )} */}
        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            marginBottom: 5,
          }}
        >
          Title
          <Add
            ghost={true}
            type={"primary"}
            hover={hover}
            icon={<PlusOutlined />}
          />
        </div>
        <ul className="list">
          {/* <TransitionGroup className="list"> */}
          {items.map((item, index) => (
            // <CSSTransition
            //   key={item.name}
            //   timeout={300}
            //   classNames="alert"
            //   unmountOnExit
            // >
            <li
              // onClick={() => {
              //   setItems(items.filter((v) => v.name != item.name))
              // }}
              className="alert li"
            >
              <Item path={`user.${index}.`} hover={hover} />
            </li>
            // </CSSTransition>
          ))}
          {/* </TransitionGroup> */}
        </ul>
      </div>
    </Formik>
  )
}
