// import { t } from "testcafe"
// import { Selector } from "testcafe"

// export async function authenticate(user: string, password: string) {
//   await t.typeText(
//     Selector("input").withAttribute("placeholder", /Email/),
//     user,
//   )

//   await t
//     .click(Selector("input").withAttribute("value", "Next"))
//     .typeText(
//       Selector("input").withAttribute("placeholder", "Password"),
//       password,
//     )
//     .click(Selector("input").withAttribute("value", "Sign in"))

//   if (await Selector("div").withText("More information required").exists) {
//     await t.click(
//       Selector("a").withText("Skip for now (14 days until this is required)"),
//     )
//   }

//   if (await Selector("div").withText("Permissions requested").exists) {
//     await t.click(Selector("input").withAttribute("value", "Accept"))
//   }

//   if (await Selector("div").withText("Stay signed in?").exists) {
//     await t.click(Selector("input").withAttribute("value", "Yes"))
//   }

//   // TODO
//   // if (await Selector("div").withText("Stay signed in").exists) {
//   //   await t.click(Selector("input").withAttribute("value", "Next"))
//   // }
// }
