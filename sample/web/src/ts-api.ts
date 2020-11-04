/* eslint-disable prettier/prettier */
import { CreatePortfolio, DeletePortfolio, Unit, UpdatePortfolio } from "./ts-models"
export module Configuration {
  export async function LatestList_v1() {
    const response = await fetch(`/api/glow-configuration/list?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
export module Configuration {
  export async function List_v1() {
    const response = await fetch(`/api/glow-configuration/all?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
export module Configuration {
  export async function Single_v1(id: string | null,version: number | null,name: string | null) {
    const response = await fetch(`/api/glow-configuration/single/${id}?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
export module Portfolios {
  export async function Create_v1(request: CreatePortfolio) {
    const response = await fetch(`/api/portfolios/create?api-version=1.0`, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(request),
    })
    const data = await response.json()
    return data
  }
}
export module Portfolios {
  export async function Delete_v1(request: DeletePortfolio) {
    const response = await fetch(`/api/portfolios/delete?api-version=1.0`, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(request),
    })
    const data = await response.json()
    return data
  }
}
export module Portfolios {
  export async function Examples_v1() {
    const response = await fetch(`/api/portfolios/examples?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
export module Portfolios {
  export async function GetList_v1() {
    const response = await fetch(`/api/portfolios/list?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
export module Portfolios {
  export async function GetSingle_v1(id: string) {
    const response = await fetch(`/api/portfolios/single/${id}?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
export module Portfolios {
  export async function StageFiles_v1(request: Unit) {
    const response = await fetch(`/api/portfolios/stage-files?api-version=1.0`, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(request),
    })
    const data = await response.json()
    return data
  }
}
export module Portfolios {
  export async function Update_v1(request: UpdatePortfolio) {
    const response = await fetch(`/api/portfolios/update?api-version=1.0`, {
      method: "POST",
      headers: { "content-type": "application/json" },
      body: JSON.stringify(request),
    })
    const data = await response.json()
    return data
  }
}
export module SampleListView {
  export async function Get_v1(skip: number | null,take: number | null) {
    const response = await fetch(`/api/list-view/data?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
export module Schema {
  export async function Get_v1() {
    const response = await fetch(`/api/configuration-schemas?api-version=1.0`)
    const data = await response.json()
    return data
  }
}
