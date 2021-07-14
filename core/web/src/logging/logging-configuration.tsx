import * as React from "react"
import { Field, FormItem, Input, Select } from "formik-antd"
import {
  LogEventLevel,
  LogEventLevelValuesArray,
} from "../ts-models/Serilog.Events"
import { StronglyTypedOptions } from "../configuration/strongly-typed-options"
import { VerticalSpace } from ".."

function SelectLoglevel({ name, label }: { name: string; label: string }) {
  return (
    <FormItem name={name} label={label}>
      <Select name={name} placeholder={label}>
        {LogEventLevelValuesArray.map((v) => (
          <Select.Option key={v} value={v}>
            {v}
          </Select.Option>
        ))}
        <Select.Option key="null" value={""}>
          reset
        </Select.Option>
      </Select>
      {/* <Input name="minimumLevel.default" placeholder="Default" /> */}
    </FormItem>
  )
}

export function LoggingConfiguration() {
  return (
    <StronglyTypedOptions
      configurationId="logging"
      url="/api/configurations/logging"
      title="Logging"
      type="children"
      children={
        <VerticalSpace>
          <SelectLoglevel name="minimumLevel.default" label="Default" />
          <SelectLoglevel name="minimumLevel.override['Glow']" label="Glow" />
          <SelectLoglevel
            name="minimumLevel.override['Microsoft']"
            label="Microsoft"
          />
          <SelectLoglevel
            name="minimumLevel.override['Microsoft.EntityFrameworkCore']"
            label="Entity Framework"
          />

          <SelectLoglevel
            name="minimumLevel.override['System']"
            label="System"
          />
        </VerticalSpace>
      }
    />
  )
}
