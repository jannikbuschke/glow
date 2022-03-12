import dayjs from "dayjs"
import { Formik } from "formik"
import { Form, Input, SubmitButton } from "formik-antd"
import { RenderObject, VerticalSpace } from "glow-core"
import * as React from "react"
import { Route, Routes } from "react-router-dom"
import { TypedForm, useTypedQuery } from "../../ts-models/api"
import {
  Divider,
  Flex,
  Grid,
  Heading,
  HStack,
  StackDivider,
  Textarea,
  toast,
  VStack,
  Wrap,
  WrapItem,
} from "@chakra-ui/react"
import { Select, Input as InputC } from "@chakra-ui/react"

import {
  Tabs,
  TabList,
  TabPanels,
  Tab,
  TabPanel,
  List,
  ListIcon,
  ListItem,
} from "@chakra-ui/react"
import { defaultStringWrapper } from "../../ts-models/Glow.Sample.Azdo"
import { Button, ButtonGroup } from "@chakra-ui/react"

export function AzureDevopsExample() {
  return (
    <Routes>
      <Route path="azdo" element={<VariableGroupView />}>
        {/* <Route path=":configurationId" element={<Detail />} /> */}
      </Route>
    </Routes>
  )
}

function SelectProject({
  onChange,
}: {
  onChange: (id: string | null) => void
}) {
  const { data } = useTypedQuery("/azdo/get-projects", {
    input: {},
    placeholder: [],
  })
  return (
    <Select
      placeholder="Select project"
      size="small"
      onChange={(e) => {
        onChange(e.target.value)
      }}
    >
      {data.map((v) => (
        <option key={v.id} value={v.id}>
          {v.name}
        </option>
      ))}
    </Select>
  )
}

export function VariableGroupView() {
  const [projectName, setProjectName] = React.useState("")
  const [path, setPath] = React.useState("")
  const { data, error, isLoading, refetch } = useTypedQuery(
    "/azdo/get-commits",
    {
      input: { projectName },
      placeholder: [],
      queryOptions: {
        initialData: [],
        enabled: Boolean(projectName),
      },
    },
  )
  const [projectId, setProjectId] = React.useState<string | null>(null)
  const [content, setContent] = React.useState<string | null>(null)
  const [branchName, setBranchname] = React.useState<string>("new-branch")

  const { data: item } = useTypedQuery("/azdo/get-item", {
    input: { path, projectId },
    placeholder: defaultStringWrapper,
    queryOptions: {
      enabled: Boolean(path),
    },
  })
  React.useEffect(() => {
    setContent(item.value)
  }, [item])

  const { data: items } = useTypedQuery("/azdo/get-items", {
    input: { projectId },
    placeholder: [],
    queryOptions: {
      enabled: Boolean(projectId),
    },
  })
  return (
    <Flex width="100%">
      <VStack spacing={4} align="stretch" width="100%">
        <div>project: {projectId}</div>
        <SelectProject onChange={(v) => setProjectId(v)} />
        <Tabs defaultIndex={2}>
          <TabList>
            <Tab>Create library</Tab>
            <Tab>Get Commits</Tab>
            <Tab>Create commit</Tab>
            <Tab>Get file content</Tab>
          </TabList>

          <TabPanels>
            <TabPanel>
              <TypedForm
                actionName="/azdo/create-library"
                initialValues={{ projectName: "glue" }}
              >
                <VerticalSpace>
                  <Input name="projectName" />
                  <SubmitButton>create library</SubmitButton>
                </VerticalSpace>
              </TypedForm>
            </TabPanel>
            <TabPanel>
              <div>
                <Formik
                  initialValues={{ projectName: "glue" }}
                  onSubmit={(values, actions) => {
                    setProjectName(values.projectName)
                    refetch()
                    actions.setSubmitting(false)
                  }}
                >
                  <Form>
                    <VerticalSpace>
                      <Input name="projectName" />
                      <Button type="submit">get commits</Button>
                    </VerticalSpace>
                  </Form>
                </Formik>
                {data.map((v) => (
                  <div key={v.gitCommit.commitId}>
                    <Heading>{v.gitCommit.comment}</Heading>
                    <RenderObject {...v.changes} />
                  </div>
                ))}
              </div>
            </TabPanel>
            <TabPanel>
              <div>
                <TypedForm
                  actionName="/azdo/create-commit"
                  initialValues={{
                    projectId,
                    path,
                    content,
                    description: "",
                    name: "",
                  }}
                  onSuccess={() =>
                    toast.notify(() => "successs", {
                      status: "success",
                    })
                  }
                >
                  {(f) => (
                    <>
                      <VStack spacing={4} align="stretch" width="100%">
                        <Grid templateColumns={"auto 1fr"} gap={8}>
                          <List>
                            {items.map((v) => (
                              <ListItem
                                style={{
                                  fontWeight:
                                    v.path === path ? "bold" : undefined,
                                }}
                                cursor="pointer"
                                key={v.originalObjectId}
                                onClick={() => setPath(v.path || "")}
                              >
                                <div>{v.path}</div>
                              </ListItem>
                            ))}
                          </List>

                          <Textarea
                            value={content || ""}
                            onChange={(v) => setContent(v.target.value)}
                          />
                        </Grid>

                        <Input name="name" placeholder="PR Name" />
                        <Textarea
                          placeholder="PR Description"
                          value={f.values.description!}
                          onChange={(v) =>
                            f.setFieldValue("description", v.target.value)
                          }
                        />

                        <Button isLoading={f.isSubmitting} type="submit">
                          Create PR
                        </Button>
                      </VStack>
                    </>
                  )}
                </TypedForm>
              </div>
            </TabPanel>
            <TabPanel>
              <Grid templateColumns={"auto 1fr"} gap={8}>
                <List>
                  {items.map((v) => (
                    <ListItem
                      style={{
                        fontWeight: v.path === path ? "bold" : undefined,
                      }}
                      cursor="pointer"
                      key={v.originalObjectId}
                      onClick={() => setPath(v.path || "")}
                    >
                      <div>
                        {v.path}
                        {/* <RenderObject {...v} /> */}
                      </div>
                    </ListItem>
                  ))}
                </List>

                <Textarea value={item.value || ""} />
              </Grid>
            </TabPanel>
          </TabPanels>
        </Tabs>
      </VStack>
    </Flex>
  )
}
