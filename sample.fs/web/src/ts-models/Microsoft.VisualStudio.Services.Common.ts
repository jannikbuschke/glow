export interface SubjectDescriptor {
  subjectType: string | null
  identifier: string | null
}

export const defaultSubjectDescriptor: SubjectDescriptor = {
  subjectType: null,
  identifier: null,
}

