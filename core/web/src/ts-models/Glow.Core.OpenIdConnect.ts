export interface SetOpenIdConnectOptions {
  tenantId: string | null
  clientId: string | null
  clientSecret: string | null
}

export const defaultSetOpenIdConnectOptions: SetOpenIdConnectOptions = {
  tenantId: null,
  clientId: null,
  clientSecret: null,
}

