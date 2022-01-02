import React, { useEffect } from "react"
import classNames from "classnames"
import type { DraggableSyntheticListeners } from "@dnd-kit/core"
import type { Transform } from "@dnd-kit/utilities"

import { Handle, Remove } from "./components"

import styles from "./Item.module.css"
import styled from "styled-components"

export interface Props {
  dragOverlay?: boolean
  color?: string
  disabled?: boolean
  dragging?: boolean
  handle?: boolean
  height?: number
  index?: number
  fadeIn?: boolean
  transform?: Transform | null
  listeners?: DraggableSyntheticListeners
  sorting?: boolean
  style?: React.CSSProperties
  transition?: string | null
  wrapperStyle?: React.CSSProperties
  value: React.ReactNode
  onRemove?(): void
  renderItem?(args: {
    dragOverlay: boolean
    dragging: boolean
    sorting: boolean
    index: number | undefined
    fadeIn: boolean
    listeners: DraggableSyntheticListeners
    ref: React.Ref<HTMLElement>
    style: React.CSSProperties | undefined
    transform: Props["transform"]
    transition: Props["transition"]
    value: Props["value"]
  }): React.ReactElement
}

export const Item = React.memo(
  React.forwardRef<HTMLLIElement, Props>(
    (
      {
        color,
        dragOverlay,
        dragging,
        disabled,
        fadeIn,
        handle,
        height,
        index,
        listeners,
        onRemove,
        renderItem,
        sorting,
        style,
        transition,
        transform,
        value,
        wrapperStyle,
        ...props
      },
      ref,
    ) => {
      useEffect(() => {
        if (!dragOverlay) {
          return
        }

        // document.querySelector(':root')?.style?.setProperty("--scale-x","1.1");
        // document.documentElement.root.style.setProperty("--scale-y","1.1");
        // document.documentElement.getRootNode().style.setProperty("--scale","1.1");
        document.body.style.cursor = "grabbing"

        return () => {
          document.body.style.cursor = ""
        }
      }, [dragOverlay])

      return renderItem ? (
        renderItem({
          dragOverlay: Boolean(dragOverlay),
          dragging: Boolean(dragging),
          sorting: Boolean(sorting),
          index,
          fadeIn: Boolean(fadeIn),
          listeners,
          ref,
          style,
          transform,
          transition,
          value,
        })
      ) : (
        <Wrapper
          className={
            classNames()
            // styles.Wrapper,
            // fadeIn && styles.fadeIn,
            // sorting && styles.sorting,
            // dragOverlay && styles.dragOverlay,
          }
          fadeIn={fadeIn}
          dragOverlay={dragOverlay}
          style={
            {
              ...wrapperStyle,
              transition,
              "--translate-x": transform
                ? `${Math.round(transform.x)}px`
                : undefined,
              "--translate-y": transform
                ? `${Math.round(transform.y)}px`
                : undefined,
              "--scale-x": transform?.scaleX
                ? `${transform.scaleX}`
                : undefined,
              "--scale-y": transform?.scaleY
                ? `${transform.scaleY}`
                : undefined,
              "--index": index,
              "--color": color,
            } as React.CSSProperties
          }
          ref={ref}
        >
          <ItemDiv
            dragOverlay={dragOverlay}
            className={classNames(
              styles.Item,
              dragging && styles.dragging,
              handle && styles.withHandle,
              dragOverlay && styles.dragOverlay,
              disabled && styles.disabled,
              color && styles.color,
            )}
            style={style}
            data-cypress="draggable-item"
            {...(!handle ? listeners : undefined)}
            {...props}
            tabIndex={!handle ? 0 : undefined}
          >
            {/* <RenderObject {...transform} /> */}
            {value}
            <span className={styles.Actions}>
              {onRemove ? (
                <Remove className={styles.Remove} onClick={onRemove} />
              ) : null}
              {handle ? <Handle {...listeners} /> : null}
            </span>
          </ItemDiv>
        </Wrapper>
      )
    },
  ),
)

const Wrapper = styled.li<{
  fadeIn: boolean | undefined
  dragOverlay: boolean | undefined
}>`
  display: flex;
  box-sizing: border-box;
  transform: translate3d(var(--translate-x, 0), var(--translate-y, 0), 0)
    scaleX(var(--scale-x, 1)) scaleY(var(--scale-y, 1));
  transform-origin: 0 0;
  touch-action: manipulation;

  animation: ${(props) => (props.fadeIn ? "fadeIn 500ms ease" : undefined)}
    //   &.fadeIn {
    //   animation: fadeIn 500ms ease;
    // }
    ${({ dragOverlay }) =>
      dragOverlay &&
      `
  --scale: 1.05;
  // --scale-x: 1.05;
  // --scale-y: 1.05;
  --box-shadow: var(--box-shadow);
  --box-shadow-picked-up: var(--box-shadow-border),
    -1px 0 15px 0 rgba(34, 33, 81, 0.01),
    0px 15px 15px 0 rgba(34, 33, 81, 0.25);

    z-index: 999;
  `}; //
  //
  // &.dragOverlay {
  //   --scale: 1.05;
  //   --box-shadow: --box-shadow;
  //   --box-shadow-picked-up: --box-shadow-border,
  //     -1px 0 15px 0 rgba(34, 33, 81, 0.01),
  //     0px 15px 15px 0 rgba(34, 33, 81, 0.25);
  //   z-index: 999;
  // }
`

const ItemDiv = styled.div<{
  dragOverlay: boolean | undefined
}>`
  position: relative;
  display: flex;
  flex-grow: 1;
  align-items: center;
  padding: 18px 20px;
  background-color: var(--background-color);
  // background-color: --background-color;
  box-shadow: var(--box-shadow);
  outline: none;
  border-radius: calc(4px / var(--scale-x, 1));
  box-sizing: border-box;
  list-style: none;
  transform-origin: 50% 50%;

  -webkit-tap-highlight-color: transparent;

  color: var(--text-color);
  font-weight: var(--font-weight);
  font-size: 1rem;
  white-space: nowrap;

  transform: scale(var(--scale, 1));
  transition: box-shadow 200ms cubic-bezier(0.18, 0.67, 0.6, 1.22);

  &:focus-visible {
    box-shadow: 0 0px 4px 1px var(--focused-outline-color), var(--box-shadow);
    background: red;
    color: red;
  }

  &:not(.withHandle) {
    touch-action: manipulation;
    cursor: grab;
  }

  &.dragging:not(.dragOverlay) {
    opacity: var(--dragging-opacity, 0.5);
    z-index: 0;

    &:focus {
      box-shadow: var(--box-shadow);
    }
  }

  &.disabled {
    color: #999;
    background-color: #f1f1f1;
    &:focus {
      box-shadow: 0 0px 4px 1px rgba(0, 0, 0, 0.1), var(--box-shadow);
    }
    cursor: not-allowed;
  }

  ${({ dragOverlay }) =>
    dragOverlay &&
    `
  cursor: inherit;
  /* box-shadow: 0 0px 6px 2px --focused-outline-color; */
  animation: pop 200ms cubic-bezier(0.18, 0.67, 0.6, 1.22);
  transform: scale(var(--scale));
  box-shadow: var(--box-shadow-picked-up);
  opacity: 1;
`}

  // &.dragOverlay {
  //   cursor: inherit;
  //   /* box-shadow: 0 0px 6px 2px --focused-outline-color; */
  //   animation: pop 200ms cubic-bezier(0.18, 0.67, 0.6, 1.22);
  //   transform: scale(var(--scale));
  //   box-shadow: var(--box-shadow-picked-up);
  //   opacity: 1;
  // }

  &.color:before {
    content: "";
    position: absolute;
    top: 50%;
    transform: translateY(-50%);
    left: 0;
    height: 100%;
    width: 3px;
    display: block;
    border-top-left-radius: 3px;
    border-bottom-left-radius: 3px;
    background-color: var(--color);
  }

  &:hover {
    .Remove {
      visibility: visible;
    }
  }
`
