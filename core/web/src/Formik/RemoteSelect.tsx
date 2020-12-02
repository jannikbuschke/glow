import * as React from "react"
import { Select as $Select } from "formik-antd"
import { FormikFieldProps } from "formik-antd/lib/FieldProps"
import { Select, Alert, Spin } from "antd"
import { debounce } from "lodash"
import { SelectProps } from "antd/lib/select"
import { useFetch } from "../http/fetch-context"

export const RemoteSelect = ({
  name,
  validate,
  fetch,
  addHeaders,
  transform,
  keySelector,
  renderItem,
  ...restProps
}: FormikFieldProps &
  SelectProps<any> & {
    fetch: (search: string) => Promise<any>
    addHeaders?: () => Promise<HeadersInit>
    transform?: (data: any) => any
    keySelector?: (data: any) => string | number
    renderItem?: (data: any) => React.ReactNode
  }) => {
  const [search, setSearch] = React.useState("")

  const [raw, setData] = React.useState<any>(null)
  const [error, setError] = React.useState("")
  React.useEffect(() => {
    ;(async () => {
      try {
        const response = await fetch(search)
        setData(response)
      } catch (e) {
        setError(e.toString())
      }
    })()
  }, [search])

  const debouncedSearch = debounce(setSearch, 500)

  if (error) {
    return <Alert type="error" showIcon={false} banner={true} message={error} />
  }

  const data = transform ? transform(raw) : raw

  return (
    <Spin spinning={data === null || data === undefined} delay={250}>
      <$Select
        name={name}
        showSearch={true}
        style={{ width: "100%" }}
        onSearch={debouncedSearch}
        allowClear={true}
        showArrow={true}
        filterOption={false}
        notFoundContent={null}
        {...restProps}
      >
        {Array.isArray(data)
          ? data.map((item: any, index) => (
              <Select.Option
                key={keySelector ? keySelector(item) : item.id || index}
                value={item.value}
              >
                {renderItem
                  ? renderItem(item)
                  : item.name || item.displayName || item.title}
              </Select.Option>
            ))
          : null}
      </$Select>
    </Spin>
  )
}
