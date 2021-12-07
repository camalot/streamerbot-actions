using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
public enum Rarity
{
  Common,
  Uncommon,
  Rare,
  Epic,
  Legendary,
  Duplicate,
  UNKNOWN
}
[Serializable]
public class PayloadData
{
  [JsonProperty("rarity")]
  [JsonConverter(typeof(StringEnumConverter))]
  public Rarity Rarity { get; set; }
  [JsonProperty("count")]
  public int Count { get; set; } = 0;
}
public class CPHInline
{
  public bool Execute()
  {
    var data = CPH.GetGlobalVar<string>("R6S_ALPHAPACK_DATA");
    var dataFilePath = CPH.GetGlobalVar<string>("R6S_ALPHAPACK_DATA_FILE");

    if (data == null || string.IsNullOrWhiteSpace(dataFilePath))
    {
      CPH.LogDebug("[IncrementAlphaPackCounter] R6S_ALPHAPACK_DATA or R6S_ALPHAPACK_DATA_FILE is null or empty");
      return false;
    }

    var rarityName = string.Empty;
    if (args.ContainsKey("input0"))
    {
      rarityName = (args["input0"] as string).ToLower();
    }
    else if (args.ContainsKey("rarity"))
    {
      rarityName = (args["rarity"] as string).ToLower();
    }
    var rarity = Rarity.UNKNOWN;
    switch (rarityName)
    {
      case "c":
      case "common":
        rarity = Rarity.Common;
        break;
      case "u":
      case "uncommon":
        rarity = Rarity.Uncommon;
        break;
      case "r":
      case "rare":
        rarity = Rarity.Rare;
        break;
      case "e":
      case "epic":
        rarity = Rarity.Epic;
        break;
      case "l":
      case "legendary":
        rarity = Rarity.Legendary;
        break;
      case "d":
      case "duplicate":
        rarity = Rarity.Duplicate;
        break;
      default:
        CPH.LogDebug("[IncrementAlphaPackCounter] Unknown rarity: " + rarityName);
        return false;
    }
    var dataObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PayloadData>>(data);
    var rarityKey = Enum.GetName(typeof(Rarity), rarity).ToLower();
    if (dataObject.ContainsKey(rarityKey))
    {
      dataObject[rarityKey].Count++;
    }

    CPH.SetArgument("R6S_RARITY", rarityKey.ToUpper());
    var fileData = Newtonsoft.Json.JsonConvert.SerializeObject(dataObject);
    CPH.SetGlobalVar("R6S_ALPHAPACK_DATA", fileData);
    File.WriteAllText(dataFilePath, fileData);
    return true;
  }
}