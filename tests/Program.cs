using System.Runtime.Loader;
using System.Reflection;
using Drinks.WeekendCollections;
using WeekendDrops.Models;
var runtime=Environment.GetEnvironmentVariable("SPT_ROOT")??throw new InvalidOperationException("SPT_ROOT is required for native assembly resolution.");
AssemblyLoadContext.Default.Resolving+=(context,name)=>{var p=System.IO.Path.Combine(runtime,name.Name+".dll");return File.Exists(p)?context.LoadFromAssemblyPath(p):null;};
var own=new CollectionSetDefinition{Id="owned",ItemIds=["item"],GpPerItem=123,Milestones=[new(){Required=1,GpReward=321,Perk=CollectionPerkKind.GpPercent,PerkValue=2}]};
var foreign=new CollectionSetDefinition{Id="foreign",ItemIds=["other"],GpPerItem=456};
var definition=new CollectionSetDefinition{Id="owned",ItemIds=["item"],Milestones=[new(){Required=1,Perk=CollectionPerkKind.GpPercent,PerkValue=10,Description="10%"}]};
var current=new List<CollectionSetDefinition>{foreign,own};
if(CollectionMerger.Merge(current,new[]{definition})!=0||own.GpPerItem!=123||own.Milestones[0].GpReward!=321||own.Milestones[0].PerkValue!=10||!ReferenceEquals(current[0],foreign))throw new Exception("Existing rewards or foreign collections changed.");
if(CollectionMerger.Merge(current,new[]{definition})!=0||current.Count!=2)throw new Exception("Reload duplicated collections.");
try{CollectionMerger.Merge(current,new[]{definition,definition});throw new Exception("Duplicate definitions accepted.");}catch(InvalidDataException){}
var fresh=new List<CollectionSetDefinition>();if(CollectionMerger.Merge(fresh,new[]{definition})!=1)throw new Exception("New collection not added.");
Console.WriteLine("PASS: 10% upgrade, cash rewards retained, foreign sets untouched, reload idempotent and duplicates rejected.");

var sets=System.Text.Json.JsonSerializer.Deserialize<List<CollectionSetDefinition>>(File.ReadAllText("src/WeekendCollections/collections.json"),new System.Text.Json.JsonSerializerOptions{Converters={new System.Text.Json.Serialization.JsonStringEnumConverter()}})!;
NativeBonus.Verify(sets);

var collectionType=typeof(WeekendDrops.Services.CollectionService);
var binding=BindingFlags.Instance|BindingFlags.NonPublic;
if(collectionType.GetMethod("Load",binding,null,new[]{typeof(string)},null)?.ReturnType!=typeof(PlayerCollectionState)||collectionType.GetMethod("Save",binding,null,new[]{typeof(string),typeof(PlayerCollectionState)},null)==null)throw new Exception("Native migration contract changed.");
Console.WriteLine("PASS: native Load/Save migration signatures match.");
