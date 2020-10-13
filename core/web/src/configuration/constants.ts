const baseUrl = "/configuration"

export const constants = {
  baseUrl,
  paths: {
    list: `${baseUrl}/`,
    id: `${baseUrl}/:id`,
    single: (id: string) => `${baseUrl}/${id}`,
  },
  api: {
    list: "/api/glow-configuration/list",
    all: "/api/glow-configuration/all",
    single: (id: string, version: string, name: string) =>
      `/api/glow-configuration/single/${id}?version=${version}&name=${name}`,
  },
}
