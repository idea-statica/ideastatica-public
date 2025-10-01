1. Download thi zip file which inclides the outpu from Devops pipeline ( _CON OpenAPI clients.zip_ or _RCS OpenAPI clients.zip_ ) to a temporary folder (e.g. _c:\temp_ )
2. Open Powershel and navigate to the directory with scripts ( _ideastatica-public\src\api-sdks_ )
3. Run

```
# Update Connection REST API Client
.\copy_con_client.ps1 -ZipFile "C:\temp\CON OpenAPI clients.zip"

# Update RCS REST API Client
.\copy_rcs_client.ps1 -ZipFile "C:\temp\RCS OpenAPI clients.zip"
```