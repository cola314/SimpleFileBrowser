import React from 'react';
import { ContextMenu, MenuItem, ContextMenuTrigger } from 'react-contextmenu';

function handleClick(e, data) {
  console.log(data.foo);
}

export default function FolderContextMenu({ id, deleteFolder, renameFolder, downloadFolder, children }) {

  return (
    <>
      <ContextMenuTrigger id={id}>
        {children}
      </ContextMenuTrigger>

      <ContextMenu id={id}>
        <MenuItem onClick={() => downloadFolder()}>zip로 다운로드</MenuItem>
        <MenuItem onClick={() => deleteFolder()}>삭제</MenuItem>
        <MenuItem onClick={() => renameFolder()}>이름 변경</MenuItem>
      </ContextMenu>
    </>
  )
}