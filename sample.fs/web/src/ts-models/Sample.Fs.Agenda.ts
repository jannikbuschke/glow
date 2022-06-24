/* eslint-disable prettier/prettier */
import { FSharpOption } from "./Microsoft.FSharp.Core"
import { defaultFSharpOption } from "./Microsoft.FSharp.Core"

export interface CreateMeeting {
}

export const defaultCreateMeeting: CreateMeeting = {
}

export interface GetListOfOptions {
}

export const defaultGetListOfOptions: GetListOfOptions = {
}

export interface GetMeeting {
  meetingId: string
  version: number
}

export const defaultGetMeeting: GetMeeting = {
  meetingId: "00000000-0000-0000-0000-000000000000",
  version: 0,
}

export interface ReorderAgendaItems {
  meetingId: string
  oldIndex: number
  newIndex: number
}

export const defaultReorderAgendaItems: ReorderAgendaItems = {
  meetingId: "00000000-0000-0000-0000-000000000000",
  oldIndex: 0,
  newIndex: 0,
}

export interface MeetingItem {
  id: string
  displayName: string | null
  duration: number
}

export const defaultMeetingItem: MeetingItem = {
  id: "00000000-0000-0000-0000-000000000000",
  displayName: null,
  duration: 0,
}

export interface UpsertMeetingItem {
  meetingId: string
  meetingItem: MeetingItem
}

export const defaultUpsertMeetingItem: UpsertMeetingItem = {
  meetingId: "00000000-0000-0000-0000-000000000000",
  meetingItem: {} as any,
}

export interface Meeting {
  id: string
  name: string | null
  items: MeetingItem[]
}

export const defaultMeeting: Meeting = {
  id: "00000000-0000-0000-0000-000000000000",
  name: null,
  items: [],
}

export interface WrappListOfOptions {
  persons: FSharpOption<Person>[]
}

export const defaultWrappListOfOptions: WrappListOfOptions = {
  persons: [],
}

export interface Person {
  firstName: string | null
  lastName: string | null
}

export const defaultPerson: Person = {
  firstName: null,
  lastName: null,
}

