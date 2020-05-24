import { Field, FormikProps } from "formik"
import * as React from "react"
import { ButtonProps } from "antd/lib/button"
import { Button,  message } from "antd"

type Props = {
  payload: any
  path: string
  onSuccess?: (value?: any) => void
  onError?: (e: any) => void
  scopes?: string[]
}

export const ActionButton = ({
  path,
  payload,
  onSuccess,
  onError,
  scopes,
  ...props
}: ButtonProps & Props) => {
  const [loading, setLoading] = React.useState(false)
  return (
    <Field>
      {({ form }: { field: any; form: FormikProps<any> }) => (
        <Button
          loading={loading && { delay: 150 }}
          onClick={async () => {
            setLoading(true)

            try {
              const response = await fetch(path, {
                method: "POST",
                body: JSON.stringify(payload),
                headers: { "content-type": "application/json" },
              })

              if (onSuccess && response.ok) {
                if (response.status === 201) {
                  const value = await response.json()
                  onSuccess(value)
                }
                if (response.status === 200) {
                  onSuccess()
                }
              }
            } catch (E) {
              message.error(E.toString())
            } finally {
              setLoading(false)
            }
          }}
          {...props}
        />
      )}
    </Field>
  )
}
