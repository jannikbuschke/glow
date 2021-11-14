namespace sample.fs

open  Microsoft.EntityFrameworkCore;

type Person =
    { PersonId : int
      FirstName : string
      LastName : string
      Address : string
      City : string}

type DataContext(options: DbContextOptions<DataContext>) =
    inherit DbContext(options)

    [<DefaultValue>]
    val mutable persons : DbSet<Person>

    member public this.Persons with get() = this.persons
                               and set p = this.persons <- p

