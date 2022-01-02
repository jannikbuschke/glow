import * as React from "react"
import { Upload, Button, notification } from "antd"
import { UploadProps } from "antd/lib/upload"
import { saveAs } from "file-saver"
import { useField } from "formik"
import { DeleteOutlined } from "@ant-design/icons"

interface IFile {
  id: string
  name: string
}

export function StageFiles({
  name,
  url,
  children,
  onChange,
  action,
  //   onFilesAdded,
  ...rest
}: React.PropsWithChildren<
  {
    name: string
    url: string
    //  onFilesAdded: (files: IFile[]) => void
  } & UploadProps
>) {
  const [{ value }, _, { setValue }] = useField<IFile[]>(name)

  return (
    <Upload.Dragger
      showUploadList={false}
      multiple={true}
      action={url}
      onChange={async (info: any) => {
        if (info.file.status === "done" && info.file.response) {
          const files = info.file.response.map((v: any) => ({
            id: v.id,
            name: v.name,
          })) as IFile[]
          setValue([...value, ...files])
          //   onFilesAdded(files)
        }
      }}
      {...rest}
    >
      {Boolean(children) ? (
        children
      ) : (
        <p className="ant-upload-hint">
          Click or drag file to this area to upload
        </p>
      )}
    </Upload.Dragger>
  )
}

async function downloadFile(path: string, filename: string, fetch: any) {
  const response = await fetch(path, { method: "GET", headers: {} })
  if (!response.ok) {
    notification.error({
      message: "Could not download file: " + response.statusText,
    })
  } else {
    const data = await response.blob()
    saveAs(data, filename)
  }
}

export function Files({
  name,
  disableDelete,
  fileUrl,
}: {
  name: string
  disableDelete?: boolean
  fileUrl: (id: string) => string
}) {
  const [{ value }, _, { setValue }] = useField<IFile[]>(name)

  return (
    <div>
      {value.map(({ name, id }, i: number) => (
        <div key={id}>
          <Button
            type="link"
            onClick={async () => {
              await downloadFile(fileUrl(id), name, fetch)
            }}
          >
            {name}
          </Button>

          {!disableDelete && (
            <Button
              icon={<DeleteOutlined />}
              style={{ border: "none", background: "none", opacity: "0.6" }}
              onClick={() => {
                const copy = [...value]
                copy.splice(i, 1)
                setValue(copy)
              }}
            />
          )}
        </div>
      ))}
    </div>
  )
}
