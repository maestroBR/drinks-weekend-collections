using WeekendDrops.Models;
namespace Drinks.WeekendCollections;
public static class CollectionMerger
{
 public static int Merge(List<CollectionSetDefinition> current,IEnumerable<CollectionSetDefinition> additional)
 {
  var definitions=additional.ToList();
  if(definitions.Select(s=>s.Id).Distinct(StringComparer.Ordinal).Count()!=definitions.Count)throw new InvalidDataException("Duplicate addon collection IDs.");
  
  int added=0;
  foreach(var definition in definitions){
   var matches=current.Where(s=>s.Id==definition.Id).ToList();
   if(matches.Count>1)throw new InvalidDataException("Duplicate configured collection: "+definition.Id);
   var existing=matches.FirstOrDefault();
   if(existing==null){current.Add(definition);added++;continue;}
   if(!existing.ItemIds.ToHashSet(StringComparer.Ordinal).SetEquals(definition.ItemIds))throw new InvalidDataException("Collection items differ: "+definition.Id);
   var milestone=existing.Milestones.Where(m=>m.Required==existing.ItemIds.Count).ToList();
   if(milestone.Count!=1)throw new InvalidDataException("Expected one completion milestone: "+definition.Id);
   milestone[0].Perk=CollectionPerkKind.GpPercent;milestone[0].PerkValue=10;
   milestone[0].Description=definition.Milestones.Single(m=>m.Required==definition.ItemIds.Count).Description;
  }
  return added;
 }
}
