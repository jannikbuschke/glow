import * as React from "react"
import { DatePicker as $DatePicker } from "antd"
import { FieldProps, useField } from "formik"
import moment from "moment"
import {
  DatePickerProps as $DatePickerProps,
  MonthPickerProps as $MonthPickerProps,
  RangePickerProps as $RangePickerProps,
  WeekPickerProps as $WeekPickerProps,
} from "antd/lib/date-picker"
import { FormikFieldProps } from "formik-antd/lib/FieldProps"
import { Field } from "formik-antd"

const {
  MonthPicker: $MonthPicker,
  RangePicker: $RangePicker,
  WeekPicker: $WeekPicker,
} = $DatePicker

export type DatePickerProps = $DatePickerProps &
  FormikFieldProps & { keepOffset?: boolean }

export function DatePickerCustom({
  name,
  validate,
  onChange,
  fast,
  keepOffset,
  onSelect,
  ...restProps
}: DatePickerProps) {
  return (
    <Field name={name} validate={validate} fast={fast}>
      {({
        field: { value },
        form: { setFieldValue, setFieldTouched },
      }: FieldProps) => (
        <>
          <$DatePicker
            value={value ? moment(value) : null}
            onChange={(date, dateString) => {
              setFieldValue(name, date ? date.toISOString(keepOffset) : null)
              setFieldTouched(name, true, false)
              onChange && onChange(date, dateString)
            }}
            onSelect={(e) => {
              setFieldValue(name, e ? e.toISOString(keepOffset) : null)
              setFieldTouched(name, true, false)
              onSelect && onSelect(e)
            }}
            {...restProps}
          />
          {JSON.stringify({ value })}
        </>
      )}
    </Field>
  )
}

export default DatePickerCustom

DatePickerCustom.MonthPicker = ({
  name,
  validate,
  onChange,
  keepOffset,
  ...restProps
}: MonthPickerProps) => (
  <Field name={name} validate={validate}>
    {({
      field: { value },
      form: { setFieldValue, setFieldTouched },
    }: FieldProps) => (
      <$MonthPicker
        value={value ? moment(value) : undefined}
        onChange={(date, dateString) => {
          setFieldValue(name, date ? date.toISOString(keepOffset) : null)
          setFieldTouched(name, true, false)
          onChange && onChange(date, dateString)
        }}
        {...restProps}
      />
    )}
  </Field>
)

DatePickerCustom.RangePicker = ({
  name,
  validate,
  onChange,
  ...restProps
}: RangePickerProps) => (
  <Field name={name} validate={validate}>
    {({
      field: { value },
      form: { setFieldValue, setFieldTouched },
    }: FieldProps) => (
      <$RangePicker
        name={name}
        value={value}
        onChange={(dates, dateStrings) => {
          setFieldValue(name, dates)
          setFieldTouched(name, true, false)
          onChange && onChange(dates, dateStrings)
        }}
        {...restProps}
      />
    )}
  </Field>
)

DatePickerCustom.WeekPicker = ({
  name,
  validate,
  onChange,
  ...restProps
}: WeekPickerProps) => (
  <Field name={name} validate={validate}>
    {({
      field: { value },
      form: { setFieldValue, setFieldTouched },
    }: FieldProps) => (
      <$WeekPicker
        name={name}
        value={value}
        onChange={(date, dateString) => {
          setFieldValue(name, date)
          setFieldTouched(name, true, false)
          onChange && onChange(date, dateString)
        }}
        {...restProps}
      />
    )}
  </Field>
)

export type WeekPickerProps = FormikFieldProps & $WeekPickerProps
export type RangePickerProps = FormikFieldProps & $RangePickerProps
export type MonthPickerProps = FormikFieldProps &
  $MonthPickerProps & { keepOffset?: boolean }
