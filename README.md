# ♻ Gomi Download Tools
- Download content from the Internet while maintaining the path hierarchy.
- It is currently at version 0.1.0 and is probably faulty.
- Please do not send large numbers of requests for content that has not been delivered by the CDN or for paths that do not exist.
- No overwriting is performed for paths that already exist.

## ◆ TODO List
https://github.com/Connect-a/GomiDownloadTools/projects?type=beta
## ◆ Usage
### ・ Enter HAR to download
```bash
.\Gomi.exe --har .\hoge.har
```
- In this case, the results are downloaded to the `hoge.har.dl` folder.
- Request headers are ignore.
- If the response is included in the HAR file, it is output from its contents.
- If multiple HAR files are dragged and dropped into the EXE, the downloaded files will be consolidated in the `hars.dl` folder.

### ・ Template Download
```bash
.\Gomi.exe --template .\sample.txt --csv .\sample.csv
```
- By entering text file and CSV file one at a time, the values are set from the CSV and batch downloaded.
- The file name can be arbitrary, and the extension can be set in txt and csv.
- Do not enclose CSV data in double-cotation marks
  - Please use URL encoding (Percent-encoding) since CSV with comma in the data is not supported.
- The ability to cross multiple CSVs and automatic setting of numeric ranges will be developed if there is demand.
#### sample.txt
```txt
https://example.com/units/{unitId}.ab
https://example.com/images/{unitId}/{a}.jpg
```
#### sample.csv
```csv
unitId,a
0001001,001
0001001,002
0001001,003
0001001,hoge
0001001,fuga
0001001,piyo
0001002,001
0001002,002
0001002,003
0001002,hoge
0001002,fuga
0001002,piyo
```

## ◆ License
MIT
