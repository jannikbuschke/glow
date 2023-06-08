import { SegmentedControl, SegmentedControlProps } from "@mantine/core"
import { RenderObject } from "glow-core"
import { useRoutes, useMatch, useLocation, useNavigate } from "react-router"

export function Nav(props: SegmentedControlProps & { matchPattern: string }) {
  const x = useMatch(props.matchPattern)
  const navigate = useNavigate()
  // const [selected,setSelected]=React.useState()
  return (
    <div>
      <RenderObject {...x} />
      <SegmentedControl
        value={x?.params.id}
        onChange={(x) => {
          navigate(x)
        }}
        // data={[
        //   { label: "React", value: "react" },
        //   { label: "Angular", value: "ng" },
        //   { label: "Vue", value: "vue" },
        //   { label: "Svelte", value: "svelte" },
        // ]}
        {...props}
      />
    </div>
  )
}
