/* eslint-disable prettier/prettier */
export interface IConfigurationMeta {
  route: string | null
  title: string | null
  id: string | null
  sectionId: string | null
}

export const defaultIConfigurationMeta: IConfigurationMeta = {
  route: null,
  title: null,
  id: null,
  sectionId: null,
}

