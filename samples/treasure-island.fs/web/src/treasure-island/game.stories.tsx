import * as React from "react"
import { ComponentStory, ComponentMeta } from "@storybook/react"
import { RenderGame } from "./game"
import sampleData from "../client/sample-data.json"
import { Game } from "../client/TreasureIsland"
import { RootProviders } from "../app"

export default {
  title: "Game",
  component: RenderGame,
} as ComponentMeta<typeof RenderGame>

const game = sampleData.initializedGame as any as Game
const Template: ComponentStory<typeof RenderGame> = (args, { argTypes }) => {
  return (
    <RootProviders>
      <div>
        <RenderGame game={game} refetch={() => {}} />
      </div>
    </RootProviders>
  )
}

export const Default = Template.bind({})
