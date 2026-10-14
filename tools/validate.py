import json,re
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def validate():
 sets=json.loads((ROOT/'src/WeekendCollections/collections.json').read_text())
 assert sets and len(sets)==len({s['id']for s in sets})
 ids=[i for s in sets for i in s['itemIds']]
 assert len(ids)==len(set(ids)) and all(re.fullmatch('[0-9a-f]{24}',i)for i in ids)
 expected=set()
 for s in sets:
  assert s['itemIds'] and len(s['itemIds'])==len(set(s['itemIds']))
  completion=[m for m in s['milestones']if m['required']==len(s['itemIds'])]
  assert len(completion)==1 and completion[0]['perk']=='GpPercent' and completion[0]['perkValue']==10
  assert all(0<m['required']<=len(s['itemIds']) and m['gpReward']>=0 for m in s['milestones'])
  expected.update({'collection.'+s['id']+'.name','collection.'+s['id']+'.desc'})
  expected.update('collection.'+s['id']+'.milestone.'+str(m['required'])for m in s['milestones'])
 for code in ['en','ge','ru','po','ch']:
  locale=json.loads((ROOT/'locales'/f'{code}.json').read_text())
  assert set(locale)==expected and all(isinstance(v,str)and v.strip()for v in locale.values())
  assert all('10%' in locale['collection.'+s['id']+'.milestone.'+str(len(s['itemIds']))] for s in sets)
 print(f"Validated {len(sets)} collections, {len(ids)} items, 10% bonuses and five languages.")
if __name__=='__main__':validate()
