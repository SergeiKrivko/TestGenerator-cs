with open("version.txt") as f:
    version = f.read().strip()


for file in ["Releases/win-x64/setup.iss", "Releases/mac-x64/TestGenerator.app/Contents/Info.plist", 
             "Releases/linux-x64/debpkg/usr/share/applications/TestGenerator.desktop",
             "TestGenerator.Core/Services/AppService.cs", "Releases/linux-x64/debpkg/DEBIAN/control"]:
    with open(file, 'r') as f:
        text = f.read()
        
    text = text.replace("{AppVersion}", version)
    
    with open(file, 'w') as f:
            f.write(text)
