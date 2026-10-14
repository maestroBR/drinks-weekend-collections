from pathlib import Path
from zipfile import ZipFile,ZIP_DEFLATED
import hashlib,json
from validate import ROOT,validate
validate()
dist=ROOT/'dist';dist.mkdir(exist_ok=True)
file=dist/'Drinks-Weekend-Collections-1.0.0.zip'
with ZipFile(file,'w',ZIP_DEFLATED)as z:
 for p in [ROOT/'src/WeekendCollections/bin/Release/net10.0/Drinks.WeekendCollections.dll',ROOT/'src/WeekendCollections/collections.json']:
  z.write(p,'SPT_Runtime/user/mods/Drinks-WeekendCollections/'+p.name)
 p=ROOT/'client/bin/Release/netstandard2.1/Drinks.WeekendCollections.Client.dll';z.write(p,'BepInEx/plugins/Drinks-WeekendCollections/'+p.name)
 for p in (ROOT/'locales').glob('*.json'):z.write(p,'BepInEx/plugins/Drinks-WeekendCollections/locales/'+p.name)
 for name in ['README.md','LICENSE','CHANGELOG.md']:z.write(ROOT/name,name)
(dist/'checksums.json').write_text(json.dumps({file.name:hashlib.sha256(file.read_bytes()).hexdigest()},indent=2)+'\n')
print(file)
