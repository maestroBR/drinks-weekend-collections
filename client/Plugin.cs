using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;

namespace Drinks.WeekendCollections.Client
{
 [BepInPlugin("project.brainiac.drinksweekendcollections.client", "Drinks Weekend Collections Languages", "1.0.0")]
 [BepInDependency("com.20fpsguy.WeekendDrops")]
 public sealed class Plugin : BaseUnityPlugin
 {
  static readonly Dictionary<string, Dictionary<string,string>> translations = new Dictionary<string, Dictionary<string,string>>();
  static MethodInfo resolve; static PropertyInfo language; Harmony harmony;
  void Awake()
  {
   try
   {
    var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    foreach(var code in new[]{"en","ge","ru","po","ch"})
     translations[code] = JsonConvert.DeserializeObject<Dictionary<string,string>>(File.ReadAllText(Path.Combine(folder,"locales",code+".json")));
    var loc = AccessTools.TypeByName("WeekendDropsUI.Services.Loc");
    language = AccessTools.Property(loc,"Language");
    resolve = AccessTools.Method(loc,"T",new[]{typeof(string)});
    var lookup = AccessTools.Method(loc,"Or",new[]{typeof(string),typeof(string)});
    var draw = AccessTools.DeclaredMethod(AccessTools.TypeByName("WeekendDropsUI.UI.ChallengePanel"),"DrawCollectionSet");
    if(language==null || resolve==null || lookup==null || draw==null) throw new MissingMemberException("WeekendDrops 1.0.7 localization API");
    harmony = new Harmony("project.brainiac.drinksweekendcollections.client");
    harmony.Patch(lookup,postfix:new HarmonyMethod(typeof(Plugin),nameof(Localize)));
    harmony.Patch(draw,prefix:new HarmonyMethod(typeof(Plugin),nameof(TranslateSet)){priority=Priority.First});
    Logger.LogInfo("Drinks collection translations loaded: en, ge, ru, po, ch.");
   }
   catch(Exception error) { harmony?.UnpatchSelf(); Logger.LogError(error); }
  }
  static string Code()=>TranslationCatalog.Normalize((string)language.GetValue(null,null));
  static void Localize(string __0, ref string __result)
  {
   if(__0==null || !__0.StartsWith("collection.set_brainiac_drinks_",StringComparison.Ordinal)) return;
   string text;
   if(translations[Code()].TryGetValue(__0,out text))__result=text;
  }
  static object Get(object value,string name)=>AccessTools.Property(value.GetType(),name).GetValue(value,null);
  static void Set(object value,string name,string text)=>AccessTools.Property(value.GetType(),name).SetValue(value,text,null);
  static void TranslateSet(object __0)
  {
   if(__0==null)return;
   var id=Get(__0,"Id") as string;
   if(id==null || !translations["en"].ContainsKey("collection."+id+".name"))return;
   resolve.Invoke(null,new object[]{"collection."+id+".name"});
   var texts=translations[Code()]; string prefix="collection."+id;
   Set(__0,"Name",texts[prefix+".name"]); Set(__0,"Description",texts[prefix+".desc"]);
   foreach(var milestone in (IEnumerable)Get(__0,"Milestones"))
   {
    string text;
    if(texts.TryGetValue(prefix+".milestone."+Get(milestone,"Required"),out text)) Set(milestone,"Description",text);
   }
  }
  void OnDestroy()=>harmony?.UnpatchSelf();
 }
}
