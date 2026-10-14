using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WeekendDrops.Models;
using WeekendDrops.Services;
public static class NativeBonus
{
 public static void Verify(List<CollectionSetDefinition> definitions)
 {
  var path=Path.Combine(Path.GetTempPath(),"weekend-bonus-check-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(path);
  try{
   var service=(CollectionService)RuntimeHelpers.GetUninitializedObject(typeof(CollectionService));
   var flags=BindingFlags.Instance|BindingFlags.NonPublic;
   typeof(CollectionService).GetField("_config",flags)!.SetValue(service,new CollectionConfig{Enabled=true,WipeWithProfile=false,Sets=definitions});
   typeof(CollectionService).GetField("_fileLock",flags)!.SetValue(service,new object());
   typeof(CollectionService).GetField("_dataDir",flags)!.SetValue(service,path);
   for(int count=0;count<=definitions.Count;count++){
    var state=new PlayerCollectionState{Donated=definitions.Take(count).SelectMany(s=>s.ItemIds).ToList()};
    File.WriteAllText(Path.Combine(path,"test.json"),JsonSerializer.Serialize(state));
    var expected=1+count*.1;
    if(Math.Abs(service.GpBonusMultiplier("test")-expected)>1e-9||service.ScaleGp("test",1000)!=(int)Math.Round(1000*expected))throw new Exception("Native Weekend GP multiplier is incorrect.");
   }
   Console.WriteLine("PASS: native Weekend reward multiplier: 0–"+definitions.Count+" completions, 10% each.");
  }finally{Directory.Delete(path,true);}
 }
}
