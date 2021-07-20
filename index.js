const express = require('express');
const fileUpload = require('express-fileupload');
const app = express();
const fs = require('fs');
const path = require('path');

const STORE_PATH = path.join(__dirname, 'store');

const backHtml = "</br><a href='/'><h1>back</h1></a>"

if (!fs.existsSync(STORE_PATH)) {
  console.log(STORE_PATH);
  fs.mkdirSync(STORE_PATH);
}

app.use(fileUpload());

app.get('/store', (req, res) => {
  res.download(path.join(STORE_PATH, req.query.path), req.query.path);
});

app.get('/', (req, res) => {
  let fileList = fs.readdirSync(STORE_PATH, (err, fileList) => {
    return fileList;
  });
  let str = '';
  for (let i = 0; i < fileList.length; i++) {
    str += "<a href=\"store?path=" + fileList[i] + "\">" + fileList[i] + "</a><br/>"
  }
  fs.readFile('index.html', function (err, data) {
    res.writeHead(200, { 'Content-Type': 'text/html' });
    return res.end(data + str);
  });
});

app.post('/delete', function (req, res) {
  let fileName = req.body.fileName;

  fs.unlink(path.join(STORE_PATH, fileName), (err) => {
    if (err) {
      res.send(err.toString() + ' ' + backHtml);
    } else {
      res.send(fileName + ' deleted' + backHtml);
    }
  })
});

app.post('/upload', function (req, res) {
  if (!req.files || Object.keys(req.files).length === 0) {
    return res.status(400).send('No files were uploaded.' + backHtml);
  }

  let sampleFile = req.files.sampleFile;

  sampleFile.mv('store//' + sampleFile.name, function (err) {
    if (err)
      return res.status(500).send(err + backHtml);

    res.send('File uploaded!' + backHtml);
  });
});


app.listen(9100, () => {
  console.log('Server start on 9100');
});
