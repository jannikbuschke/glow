export interface SetSecret {
  secretName: string | null
  secretValue: string | null
}

export const defaultSetSecret: SetSecret = {
  secretName: null,
  secretValue: null,
}

export interface RestartApplication {
}

export const defaultRestartApplication: RestartApplication = {
}

