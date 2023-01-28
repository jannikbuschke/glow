namespace MyNamespace1

type MyType1 = { Id: string }

type Generic<'a> = { Id: 'a }

namespace MyNamespace2

open MyNamespace1

type MyType2 =
  { Id: string
    Items: MyType1 list
    Option: Option<Generic<string>> }
