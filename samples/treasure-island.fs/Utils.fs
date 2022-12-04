namespace TreasureIsland

open System

module Utils =

  let random = Random()

  let RandomInt max = random.Next(max)

  let Chance x = random.NextDouble() < x

  // new Item("Coin", "🍕🍔🍟🌭🍿🥓🍓🍅🥭🍎🍉🍄🌶🍀❤🧡💛💚💥💢💫⛏🔨🪓🗡⚔🔪🏹🛡💣"),
  let items: Item list =
    [ { Name = "Sword"
        Item.Icon = "🗡"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 3 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Sword"
        Item.Icon = "🗡"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 3 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Sword"
        Item.Icon = "🗡"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 3 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Sword"
        Item.Icon = "🗡"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 3 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Axe"
        Item.Icon = "🪓"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 2 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Double Sword"
        Item.Icon = "🔪"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 2 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Bow"
        Item.Icon = "🏹"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 5 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Knife"
        Item.Icon = "⚔"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 5 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Axe"
        Item.Icon = "🪓"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 2 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Axe"
        Item.Icon = "🪓"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 2 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Axe"
        Item.Icon = "🪓"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 2 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Amulet"
        Item.Icon = "🥇"
        Item.Regeneration = 3
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Amulet"
        Item.Icon = "🥈"
        Item.Regeneration = 2
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Amulet"
        Item.Icon = "🥉"
        Item.Regeneration = 1
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Shield"
        Item.Icon = "🛡"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 3 } }
      { Name = "Shield"
        Item.Icon = "⛓"
        Item.Regeneration = 0
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 3 } }
      { Name = "Amulet"
        Item.Icon = "🏅"
        Item.Regeneration = 1
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Amulet"
        Item.Icon = "🎖"
        Item.Regeneration = 2
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 0 } }
      { Name = "Amulet"
        Item.Icon = "💍"
        Item.Regeneration = 3
        Item.AttackModifier = { BaseAttack = 0 }
        Item.Protection = { BaseDamageReduction = 0 } } ]

  let GetRandomItem () =
    let pos = random.Next() % items.Length
    items.[pos]

  let Get () = items
