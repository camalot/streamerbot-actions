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
  Duplicate
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
public class InitData : MarshalByRefObject {
  [JsonProperty("common")]
  public PayloadData Common { get; set; } = new PayloadData() { Rarity = Rarity.Common };
  [JsonProperty("uncommon")]
  public PayloadData Uncommon { get; set; } = new PayloadData() { Rarity = Rarity.Uncommon };
  [JsonProperty("rare")]
  public PayloadData Rare { get; set; } = new PayloadData() { Rarity = Rarity.Rare };
  [JsonProperty("epic")]
  public PayloadData Epic { get; set; } = new PayloadData() { Rarity = Rarity.Epic };
  [JsonProperty("legendary")]
  public PayloadData Legendary { get; set; } = new PayloadData() { Rarity = Rarity.Legendary };
  [JsonProperty("duplicate")]
  public PayloadData Duplicate { get; set; } = new PayloadData() { Rarity = Rarity.Duplicate };
}
public class CPHInline
{
  public bool Execute()
  {
    var dataFile = CPH.GetGlobalVar<string>("R6S_ALPHAPACK_DATA_FILE", true);

    if (string.IsNullOrWhiteSpace(dataFile) || !File.Exists(dataFile)) {
      CPH.LogDebug("[InitAlphaPackCounter] R6S_ALPHAPACK_DATA_FILE is not set or file does not exist.");
      File.CreateText(dataFile).Close();
    }

    var dataLines = System.IO.File.ReadAllText(dataFile).Trim();

    var data = new InitData();
    if (!string.IsNullOrWhiteSpace(dataLines)) {
      CPH.LogDebug("[InitAlphaPackCounter] data found.");
      CPH.LogDebug("[InitAlphaPackCounter] data: " + dataLines);
      data = Newtonsoft.Json.JsonConvert.DeserializeObject<InitData>(dataLines);
    }

    CPH.SetGlobalVar("R6S_ALPHAPACK_DATA", Newtonsoft.Json.JsonConvert.SerializeObject(data));
    CPH.LogDebug("[InitAlphaPackCounter] Global Variable R6S_ALPHAPACK_DATA set.");

    return true;
  }
}