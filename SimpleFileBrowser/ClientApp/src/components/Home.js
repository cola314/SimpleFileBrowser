import React, { Component, useState, useEffect } from 'react';
import axios from 'axios';
import FolderContextMenu from './FolderContextMenu';
import FileContextMenu from './FileContextMenu';
import ParentContextMenu from './ParentContextMenu';
import * as FileSizeFormatter from '../utils/FileSizeFormatter';
import url from 'url';
import querystring from 'querystring';
import $ from 'jquery';
import './Home.css';

//HACK: jquery modal function을 쓰기 위한 과정
//https://stackoverflow.com/questions/28241912/bootstrap-modal-in-react-js
window.jQuery = $;
window.$ = $;
global.jQuery = $;

const API_SERVER = window.location.origin;
const ROOT_DIR = "storage";

export function Home() {
  const [currentPath, setCurrentPath] = useState(ROOT_DIR);
  const [files, setFiles] = useState([]);
  const [folders, setFolders] = useState([]);
  const [hashModal, setHashModal] = useState(false);
  const [fileHash, setFileHash] = useState();
  const [hashModalTitle, setHashModalTitle] = useState("")

  useEffect(() => {
    const query = querystring.parse(url.parse(window.location.href).query);
    if (query['path']) {
      setCurrentPath(query['path']);
    }
  }, [])
  useEffect(() => {
    window.history.replaceState(null, null, `?path=${encodeURI(currentPath)}`)
    loadDirectory();
  }, [currentPath])

  const loadDirectory = () => {
    axios.get(`${API_SERVER}/api/files/${encodeURI(currentPath)}`)
      .then(e => setFiles(e.data))
      .catch(err => console.error(err));

    axios.get(`${API_SERVER}/api/folders/${encodeURI(currentPath)}`)
      .then(e => setFolders(e.data))
      .catch(err => console.error(err));

  }
  const deleteFileFolder = (fileFolder) => {
    if (window.confirm(`${fileFolder.name} 삭제하시겠습니까?`)) {
      axios.post(`${API_SERVER}/api/delete/${encodeURI(fileFolder.fullName)}`)
        .then(_ => loadDirectory())
        .catch(err => {
          console.error(err);
          alert(`${fileFolder.name} 삭제에 실패했습니다`);
        });
    }
  }
  const renameFileFolder = (fileFolder) => {
    var name = window.prompt(`변경할 이름을 입력하세요`, fileFolder.name);
    if (name) {
      axios.post(`${API_SERVER}/api/rename/${encodeURI(fileFolder.fullName)}/${encodeURI(name)}`)
        .then(_ => loadDirectory())
        .catch(err => {
          console.error(err);
          alert(`이름 변경에 실패했습니다`);
        });
    }
  }
  const showFileHash = (file) => {
    axios.get(`${API_SERVER}/api/hash/${encodeURI(file.fullName)}`)
      .then(e => {
        const hash = e.data;
        setFileHash(hash);
        setHashModalTitle(`${file.name} 파일 해시 값`);
        $('#exampleModal').modal('show');
      })
      .catch(err => {
        console.log(err);
        alert(`파일 해시를 가져오는데 실패했습니다`);
      })
  }
  const CreateFolder = () => {
    const folderName = prompt(`생성할 폴더 이름을 입력하세요`);
    if (folderName) {
      axios.post(`${API_SERVER}/api/folder/${encodeURI(currentPath)}/${encodeURI(folderName)}`)
        .then(_ => loadDirectory())
        .catch(err => {
          console.error(err);
          alert(`폴더 생성에 실패했습니다`);
        })
    }
  }
  const downloadFile = (file) => {
    window.open(`${API_SERVER}/api/downloadFile/${encodeURI(file.fullName)}`);
  }
  const downloadFolder = (folder) => {
    window.open(`${API_SERVER}/api/downloadFolder/${encodeURI(folder.fullName)}`);
  }
  const onSaveFiles = (e) => {
    const formData = new FormData();
    const files = Array.prototype.slice.call(e.target.files);
    console.log(files);
    if (files.length === 0) {
      return;
    }

    files.forEach(file => {
      console.log(file);
      formData.append('multipartFiles', file);
    });

    axios.post(`${API_SERVER}/api/upload/${encodeURI(currentPath)}`, formData)
      .then(_ => loadDirectory())
      .catch(err => {
        console.error(err);
        alert('파일 업로드에 실패했습니다');
      })
  }
  const onFileUpload = () => {
    $('#fileInput').click();
  }
  const copyToClipboard = (value) => {
    const t = document.createElement("textarea");
    document.body.appendChild(t);
    t.value = value;
    t.select();
    document.execCommand('copy');
    document.body.removeChild(t);
  };
  const copyFileDownloadPath = (file) => {
    copyToClipboard(`${API_SERVER}/api/downloadFile/${encodeURI(file.fullName)}`);
    //컨텍스트 메뉴 닫히고 알림창이 뜨게함
    setTimeout(() => alert('클립보드에 복사됬습니다'), 50);
  }
  const copyFolderDownloadPath = (folder) => {
    copyToClipboard(`${API_SERVER}/api/downloadFolder/${encodeURI(folder.fullName)}`);
    //컨텍스트 메뉴 닫히고 알림창이 뜨게함
    setTimeout(() => alert('클립보드에 복사됬습니다'), 50);
  }
  return (
    <div className="mt-4">

      <div className="modal fade" id="exampleModal" tabIndex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div className="modal-dialog modal-lg">
          <div className="modal-content">
            <div className="modal-header">
              <h5 className="modal-title" id="exampleModalLabel">{hashModalTitle}</h5>
              <button type="button" className="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div className="modal-body" style={{ wordBreak: 'break-all' }}>
              <p>{fileHash && "MD5 : " + fileHash.md5}</p>
              <p>{fileHash && "SHA1 : " + fileHash.sha1}</p>
              <p>{fileHash && "SHA256 : " + fileHash.sha256}</p>
            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-primary" data-bs-dismiss="modal">확인</button>
            </div>
          </div>
        </div>
      </div>

      <input type="file" id="fileInput" multiple onChange={onSaveFiles} style={{ display: 'none' }} />
      <button className="btn btn-primary" onClick={onFileUpload}>파일 업로드</button>
      <button className="btn btn-success ml-2" onClick={CreateFolder}>폴더 추가</button>
      <p className="mt-4">현재 경로 : \{currentPath}</p>
      <table className="table mt-4 context-table">
        <thead className="thead-light">
          <tr>
            <th>이름</th>
            <th>수정한 날짜</th>
            <th className="text-right">크기</th>
          </tr>
        </thead>
        <tbody>
          {currentPath !== ROOT_DIR && (
            <tr key="..." onDoubleClick={() => setCurrentPath(currentPath.split("\\").slice(0, -1).join("\\"))}>
              <td>
                <ParentContextMenu
                  id={'...name'}
                  moveDir={() => setCurrentPath(currentPath.split("\\").slice(0, -1).join("\\"))}>
                  ...
                </ParentContextMenu>
              </td>
              <td>
                <ParentContextMenu
                  id={'...last'}
                  moveDir={() => setCurrentPath(currentPath.split("\\").slice(0, -1).join("\\"))} />
              </td>
              <td>
                <ParentContextMenu
                  id={'...size'}
                  moveDir={() => setCurrentPath(currentPath.split("\\").slice(0, -1).join("\\"))} />
              </td>
            </tr>
          )}
          {folders.map(folder => (
            <tr key={folder.name} onDoubleClick={() => setCurrentPath(currentPath + '\\' + folder.name)}>
              <td>
                <FolderContextMenu
                  id={folder.name + 'name'}
                  moveDir={() => setCurrentPath(currentPath + '\\' + folder.name)}
                  copyDownloadPath={() => copyFolderDownloadPath(folder)}
                  deleteFolder={() => deleteFileFolder(folder)}
                  renameFolder={() => renameFileFolder(folder)}
                  downloadFolder={() => downloadFolder(folder)}>
                  <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" fill="currentColor" class="bi bi-folder-fill mr-2" viewBox="0 0 16 16">
                    <path d="M9.828 3h3.982a2 2 0 0 1 1.992 2.181l-.637 7A2 2 0 0 1 13.174 14H2.825a2 2 0 0 1-1.991-1.819l-.637-7a1.99 1.99 0 0 1 .342-1.31L.5 3a2 2 0 0 1 2-2h3.672a2 2 0 0 1 1.414.586l.828.828A2 2 0 0 0 9.828 3zm-8.322.12C1.72 3.042 1.95 3 2.19 3h5.396l-.707-.707A1 1 0 0 0 6.172 2H2.5a1 1 0 0 0-1 .981l.006.139z" />
                  </svg>
                  {folder.name}
                </FolderContextMenu>
              </td>
              <td>
                <FolderContextMenu
                  id={folder.name + 'last'}
                  moveDir={() => setCurrentPath(currentPath + '\\' + folder.name)}
                  copyDownloadPath={() => copyFolderDownloadPath(folder)}
                  deleteFolder={() => deleteFileFolder(folder)}
                  renameFolder={() => renameFileFolder(folder)}
                  downloadFolder={() => downloadFolder(folder)}>
                  {folder.lastModifiedDate}
                </FolderContextMenu>
              </td>
              <td className="text-right">
                <FolderContextMenu
                  id={folder.name + 'size'}
                  moveDir={() => setCurrentPath(currentPath + '\\' + folder.name)}
                  copyDownloadPath={() => copyFolderDownloadPath(folder)}
                  deleteFolder={() => deleteFileFolder(folder)}
                  renameFolder={() => renameFileFolder(folder)}
                  downloadFolder={() => downloadFolder(folder)}>
                  -
                </FolderContextMenu>
              </td>
            </tr>
          ))}
          {files.map(file => (
            <tr key={file.name}>
              <td>
                <FileContextMenu
                  id={file.name + 'name'}
                  deleteFile={() => deleteFileFolder(file)}
                  renameFile={() => renameFileFolder(file)}
                  showFileHash={() => showFileHash(file)}
                  downloadFile={() => downloadFile(file)}
                  copyDownloadPath={() => copyFileDownloadPath(file)}>
                  <svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" fill="currentColor" class="bi bi-file-earmark mr-2" viewBox="0 0 16 16">
                    <path d="M14 4.5V14a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2a2 2 0 0 1 2-2h5.5L14 4.5zm-3 0A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V4.5h-2z" />
                  </svg>
                  {file.name}
                </FileContextMenu>
              </td>
              <td>
                <FileContextMenu
                  id={file.name + 'last'}
                  deleteFile={() => deleteFileFolder(file)}
                  renameFile={() => renameFileFolder(file)}
                  showFileHash={() => showFileHash(file)}
                  downloadFile={() => downloadFile(file)}
                  copyDownloadPath={() => copyFileDownloadPath(file)}>
                  {file.lastModifiedDate}
                </FileContextMenu>
              </td>
              <td className="text-right">
                <FileContextMenu
                  id={file.name + 'size'}
                  deleteFile={() => deleteFileFolder(file)}
                  renameFile={() => renameFileFolder(file)}
                  showFileHash={() => showFileHash(file)}
                  downloadFile={() => downloadFile(file)}
                  copyDownloadPath={() => copyFileDownloadPath(file)}>
                  {FileSizeFormatter.convertToHumanFileSize(file.size, true)}
                </FileContextMenu>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
