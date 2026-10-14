using System;
namespace Drinks.WeekendCollections.Client
{
 public static class TranslationCatalog
 {
  public static string Normalize(string language)
  {
   var code=(language??"en").ToLowerInvariant();
   if(code=="de" || code=="ge" || code.StartsWith("de-"))return "ge";
   if(code=="pt" || code=="po" || code.StartsWith("pt-"))return "po";
   if(code=="zh" || code=="ch" || code.StartsWith("zh-"))return "ch";
   if(code=="ru" || code.StartsWith("ru-"))return "ru";
   return "en";
  }
 }
}
