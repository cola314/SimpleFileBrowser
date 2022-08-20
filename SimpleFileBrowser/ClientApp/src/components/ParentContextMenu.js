import React from 'react';
import { ContextMenu, MenuItem, ContextMenuTrigger } from 'react-contextmenu';


export default function FileContextMenu({ id, moveDir, children }) {

  return (
    <>
      <ContextMenuTrigger id={id} holdToDisplay={0}>
        {children}
      </ContextMenuTrigger>

      <ContextMenu id={id}>
        <MenuItem onClick={() => moveDir()}>이동</MenuItem>
      </ContextMenu>
    </>
  )
}