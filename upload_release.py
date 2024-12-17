import os
import sys

import requests
from github import Github, Auth

path, runtime = sys.argv[1], sys.argv[2]

arch = runtime.split('-')[1]
if arch == 'x64':
    arch = 'amd64'

with open("version.txt", 'r', encoding='utf-8') as f:
    version = f.read().strip()

print(f"Version = {repr(version)}")

for root, _, files in os.walk(f'TestGenerator/bin/Release/net8.0/{runtime}'):
    for file in files:
        resp = requests.post(f"https://testgenerator-api.nachert.art/api/v1/releases?version={version}&runtime={runtime}",
                             files={'file': open(os.path.join(root, file), 'rb')},
                             headers={'Authorization': f'Bearer {os.getenv("TESTGEN_TOKEN")}'})
        if not resp.ok:
            exit(1)

# using an access token
auth = Auth.Token(os.getenv("GITHUB_TOKEN"))

# Public Web GitHub
g = Github(auth=auth)

repo = g.get_repo('SergeiKrivko/TestGenerator-cs')

release = repo.get_latest_release()
print(repr(release.tag_name), version)
if release.tag_name != "v" + version:
    release = repo.create_git_release("v" + version, f"Version {version}", '')

release.upload_asset(path, name=f"testgenerator_{version}_{arch}.{path.split('.')[-1]}")
