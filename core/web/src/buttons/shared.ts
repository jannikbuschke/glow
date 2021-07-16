import { Result } from "../Forms/use-submit"

export type ActionProps<Input, Output> = {
  url: string
  input: Input
  onResult?: (result: Result<Output>) => void
  onSuccess?: (output: Output) => void
  onErrorResult?: (error: any) => void
}
