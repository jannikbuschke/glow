namespace MyNamespace1

type MyType1 = { Id: string }

namespace MyNamespace2

open MyNamespace1

type MyType2 = { Id: string; Items: MyType1 list }
