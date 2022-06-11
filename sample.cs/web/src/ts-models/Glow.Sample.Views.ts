export interface ListViewItem {
  id: string
  displayName: string | null
  birthday: string
  city: string | null
}

export const defaultListViewItem: ListViewItem = {
  id: "00000000-0000-0000-0000-000000000000",
  displayName: null,
  birthday: "1/1/0001 12:00:00 AM",
  city: null,
}

