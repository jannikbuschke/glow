namespace TreasureIsland

open System

module Utils1 =

  let random = Random()

  let RandomInt max = random.Next(max)

  let Chance x = random.NextDouble() < x
