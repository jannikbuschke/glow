export interface INavBarItem {
  displayName: string
  path: string
}

export interface IEntityItem {
  path: string
  detail: React.ComponentType<any> | React.ComponentType<any> | any
  list: React.ComponentType<any> | React.ComponentType<any> | any
  create?: React.ComponentType<any> | React.ComponentType<any> | any
}

export interface ILinkItem {
  displayName: string
  path: string
}
