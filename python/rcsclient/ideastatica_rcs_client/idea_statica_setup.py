# module idea_statica_setup.py

import winreg
import socket

def get_ideasetup_path(version):
    key = winreg.ConnectRegistry(None, winreg.HKEY_LOCAL_MACHINE)

    subKey = winreg.OpenKey(key, f'SOFTWARE\IDEAStatiCa\{version}\IDEAStatiCa\Designer', 0, winreg.KEY_READ | winreg.KEY_WOW64_64KEY)
    if subKey is None:
       raise ValueError("IDEAStatica was not fount in Windows Registry")
    
    installDir = winreg.QueryValueEx(subKey, "InstallDir64")
 
    winreg.CloseKey(key)

    if installDir is None:
        raise ValueError("InstallDir64 was not found in Windows Registry")

    return installDir[0]

def get_free_port():
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind(('localhost', 0))
        return s.getsockname()[1]