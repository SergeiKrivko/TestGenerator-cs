import hashlib
import os
import sys
import zipfile

import requests
from github import Github, Auth

URL = "https://testgenerator-api.nachert.art/api/v1"
# URL = "http://localhost:5255/api/v1"

path, runtime = sys.argv[1], sys.argv[2]

arch = runtime.split('-')[1]
if arch == 'x64':
    arch = 'amd64'

with open("version.txt", 'r', encoding='utf-8') as f:
    version = f.read().strip()

print(f"Version = {repr(version)}")

file_models = []
publish_dir = f'C:TestGenerator/bin/Release/net8.0/{runtime}'
# publish_dir = fr'C:\Users\sergi\RiderProjects\TestGenerator\TestGenerator/bin/Release/net8.0/{runtime}/publish'
for root, _, files in os.walk(publish_dir):
    for file in files:
        file_models.append({
            'filename': os.path.relpath(os.path.join(root, file), publish_dir),
            'hash': hashlib.sha256(open(os.path.join(root, file), 'rb').read()).hexdigest()
        })

resp = requests.post(
    f"{URL}/releases/filter?runtime={runtime}",
    json=file_models,
    headers={'Authorization': f'Bearer {os.getenv("TESTGEN_TOKEN")}'}
)
print('Filter:', resp.status_code)
print(resp.text, flush=True)
if resp.status_code >= 400:
    exit(1)

files_to_push = resp.json()['data']
with zipfile.ZipFile('temp.zip', 'w', compression=zipfile.ZIP_DEFLATED) as zipf:
    for file in files_to_push:
        zipf.write(os.path.join(publish_dir, file), file)

resp = requests.post(
    f"{URL}/releases/upload?version={version}&runtime={runtime}",
    multipart={'files': [m['filename'] for m in file_models], 'zip': open('temp.zip', 'rb')},
    headers={'Authorization': f'Bearer {os.getenv("TESTGEN_TOKEN")}'}
)
print('Upload:', resp.status_code)
print(resp.text)
if resp.status_code >= 400:
    exit(1)

auth = Auth.Token(os.getenv("GITHUB_TOKEN"))
g = Github(auth=auth)

repo = g.get_repo('SergeiKrivko/TestGenerator-cs')

release = repo.get_latest_release()
print(repr(release.tag_name), version)
if release.tag_name != "v" + version:
    release = repo.create_git_release("v" + version, f"Version {version}", '')

release.upload_asset(path, name=f"testgenerator_{version}_{arch}.{path.split('.')[-1]}")
