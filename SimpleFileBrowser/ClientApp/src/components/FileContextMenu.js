import React from 'react';
import { ContextMenu, MenuItem, ContextMenuTrigger } from 'react-contextmenu';


export default function FileContextMenu({ id, deleteFile, renameFile, showFileHash, downloadFile, children }) {

  return (
    <>
      <ContextMenuTrigger id={id}>
        {children}
      </ContextMenuTrigger>

      <ContextMenu id={id}>
        <MenuItem onClick={() => downloadFile()}>다운로드</MenuItem>
        <MenuItem onClick={() => deleteFile()}>삭제</MenuItem>
        <MenuItem onClick={() => renameFile()}>이름 변경</MenuItem>
        <MenuItem onClick={() => showFileHash()}>해시 정보</MenuItem>
      </ContextMenu>
    </>
  )
}