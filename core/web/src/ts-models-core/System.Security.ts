export type SecurityRuleSet = "None" | "Level1" | "Level2"
export const defaultSecurityRuleSet = "None"
export const SecurityRuleSetValues: { [key in SecurityRuleSet]: SecurityRuleSet } = {
  None: "None",
  Level1: "Level1",
  Level2: "Level2",
}
export const SecurityRuleSetValuesArray: SecurityRuleSet[] = Object.keys(SecurityRuleSetValues) as SecurityRuleSet[]

