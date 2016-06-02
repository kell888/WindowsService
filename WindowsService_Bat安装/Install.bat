C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe WindowsService.exe
Net Start Windows_Service
sc config Windows_Service start= auto