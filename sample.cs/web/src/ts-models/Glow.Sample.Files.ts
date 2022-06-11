export interface Portfolio {
  id: string
  displayName: string | null
  files: PortfolioFile[]
  rowVersion: string | null
}

export const defaultPortfolio: Portfolio = {
  id: "00000000-0000-0000-0000-000000000000",
  displayName: null,
  files: [],
  rowVersion: null,
}

export interface PortfolioFile {
  id: string
  name: string | null
  path: string | null
}

export const defaultPortfolioFile: PortfolioFile = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
  path: null,
}

export interface CreatePortfolio {
  displayName: string | null
  files: PutPortfolioFile[]
}

export const defaultCreatePortfolio: CreatePortfolio = {
  displayName: null,
  files: [],
}

export interface PutPortfolioFile {
  id: string
  name: string | null
}

export const defaultPutPortfolioFile: PutPortfolioFile = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
}

export interface DeletePortfolio {
  id: string
}

export const defaultDeletePortfolio: DeletePortfolio = {
  id: "00000000-0000-0000-0000-000000000000",
}

