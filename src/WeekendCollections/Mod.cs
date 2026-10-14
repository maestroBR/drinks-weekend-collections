using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using HarmonyLib;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Tables;
using WeekendDrops.Models;
using WeekendDrops.Services;
namespace Drinks.WeekendCollections;

[Injectable(TypePriority=OnLoadOrder.PostLoad+1000)]
public sealed class Mod(CollectionService collection,TemplateTable templates,ISptLogger<Mod> logger) : IOnLoad
{
 private static Mod? active;
 private List<CollectionSetDefinition> definitions=[];
 private static readonly JsonSerializerOptions JsonOptions=new()
 {
  PropertyNameCaseInsensitive=true,Converters={new JsonStringEnumConverter()}
 };
 private static readonly FieldInfo ConfigField=AccessTools.Field(typeof(CollectionService),"_config")
  ?? throw new MissingFieldException("WeekendDrops CollectionService._config");

 public Task OnLoadAsync(CancellationToken cancellationToken)
 {
  cancellationToken.ThrowIfCancellationRequested();
  string root=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
  definitions=JsonSerializer.Deserialize<List<CollectionSetDefinition>>(File.ReadAllText(Path.Combine(root,"collections.json")),JsonOptions)
   ?? throw new InvalidDataException("collections.json está vazio.");
  if(definitions.Count!=10||definitions.Select(s=>s.Id).Distinct().Count()!=10)
   throw new InvalidDataException("Addon collection count or IDs are invalid.");
  foreach(var set in definitions)
  {
   if(set.ItemIds.Count==0||set.ItemIds.Distinct().Count()!=set.ItemIds.Count||set.Milestones.Any(m=>m.Required<=0||m.Required>set.ItemIds.Count))
    throw new InvalidDataException("Invalid collection: "+set.Id);
   if(set.ItemIds.Any(id=>!templates.Items.ContainsKey(new MongoId(id))))
    throw new InvalidDataException("Required item mod is missing or incompatible.");
  }
  active=this;
  var harmony=new Harmony("project.brainiac.drinksweekendcollections");
  harmony.Patch(AccessTools.Method(typeof(CollectionService),nameof(CollectionService.LoadConfig),new[]{typeof(string)}),
   postfix:new HarmonyMethod(typeof(Mod),nameof(AfterReload)));
  Merge(collection);
  return Task.CompletedTask;
 }
 private static void AfterReload(CollectionService __instance)=>active?.Merge(__instance);
 private void Merge(CollectionService service)
 {
  var config=(CollectionConfig)ConfigField.GetValue(service)!;
  int count=CollectionMerger.Merge(config.Sets,definitions);
  logger.Info($"[Brainiac Weekend] {count} collections added; owned completion bonuses set to 10%. Weekend configuration stays on disk.");
 }
}
