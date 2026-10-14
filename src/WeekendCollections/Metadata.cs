using SPTarkov.Server.Core.Models.Spt.Mod;
namespace Drinks.WeekendCollections;
public sealed class Metadata : IModMetadata
{
 public string ModGuid { get; init; }="project.brainiac.drinksweekendcollections";
 public string Name { get; init; }="Drinks Weekend Collections";
 public string Author { get; init; }="Project Brainiac";
 public List<string>? Contributors { get; init; }
 public SemanticVersioning.Version Version { get; init; }=new("1.0.0");
 public SemanticVersioning.Range SptVersion { get; init; }=new("=4.1.6");
 public bool HasPrepatcher { get; init; }
 public List<string>? Incompatibilities { get; init; }
 public Dictionary<string,SemanticVersioning.Range>? ModDependencies { get; init; }=new()
 {
  ["com.hood.moreenergydrinks"]=new(">=1.3.0 <2.0.0"),
  ["com.20fpsguy.WeekendDrops"]=new(">=1.0.7 <1.1.0")
 };
 public string? Url { get; init; }="https://github.com/maestroBR/drinks-weekend-collections";
 public string License { get; init; }="MIT";
}
