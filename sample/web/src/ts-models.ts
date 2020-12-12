
/* this file is autogenerated. Do not edit. */

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

export interface Portfolio {
  id: string
  displayName: string | null
  files: PortfolioFile[]
}

export const defaultPortfolio: Portfolio = {
  id: "00000000-0000-0000-0000-000000000000",
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

export interface CreatePortfolio {
  displayName: string | null
  files: PutPortfolioFile[]
}

export const defaultCreatePortfolio: CreatePortfolio = {
  displayName: null,
  files: [],
}

export interface DeletePortfolio {
  id: string
}

export const defaultDeletePortfolio: DeletePortfolio = {
  id: "00000000-0000-0000-0000-000000000000",
}

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

export interface KeyValuePair2StringString {
  key: string | null
  value: string | null
}

export const defaultKeyValuePair2StringString: KeyValuePair2StringString = {
  key: null,
  value: null,
}

export interface Profile {
  displayName: string | null
  id: string | null
  email: string | null
  identityName: string | null
  isAuthenticated: boolean
  objectId: string | null
  userId: string | null
  scopes: string | null[]
  claims: KeyValuePair2StringString[]
}

export const defaultProfile: Profile = {
  displayName: null,
  id: null,
  email: null,
  identityName: null,
  isAuthenticated: false,
  objectId: null,
  userId: null,
  scopes: [],
  claims: [],
}

export interface Nested {
  value: string | null
}

export const defaultNested: Nested = {
  value: null,
}

export interface Enum {
}

export const defaultEnum: Enum = {
}

export interface Nullable1Enum {
  hasValue: boolean
  value: Enum
}

export const defaultNullable1Enum: Nullable1Enum = {
  hasValue: false,
  value: defaultEnum,
}

export interface SampleConfiguration {
  prop1: string | null
  prop2: number
  nested: Nested
  enum: Enum
  nullableEnum: Nullable1Enum
}

export const defaultSampleConfiguration: SampleConfiguration = {
  prop1: null,
  prop2: 0,
  nested: defaultNested,
  enum: defaultEnum,
  nullableEnum: defaultNullable1Enum,
}

export interface User {
  id: string | null
  displayName: string | null
  email: string | null
}

export const defaultUser: User = {
  id: null,
  displayName: null,
  email: null,
}

export interface CreateUser {
  displayName: string | null
  email: string | null
}

export const defaultCreateUser: CreateUser = {
  displayName: null,
  email: null,
}

export interface SampleAction {
  foo: string | null
}

export const defaultSampleAction: SampleAction = {
  foo: null,
}

