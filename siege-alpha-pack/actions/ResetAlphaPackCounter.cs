using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
public enum Rarity {
  Common,
  Uncommon,
  Rare,
  Epic,
  Legendary,
  Duplicate
}
[Serializable]
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

public class PayloadData {
  [JsonProperty("rarity")]
  [JsonConverter(typeof(StringEnumConverter))]
  public Rarity Rarity { get; set; }
  [JsonProperty("count")]
  public int Count { get; set; } = 0;
}
public class CPHInline {
  public bool Execute() {
    var dataFilePath = CPH.GetGlobalVar<string>("R6S_ALPHAPACK_DATA_FILE");

    if (string.IsNullOrWhiteSpace(dataFilePath)) {
      CPH.LogDebug("[IncrementAlphaPackCounter] R6S_ALPHAPACK_DATA_FILE is null or empty");
      return false;
    }
    var data = new InitData();

    var fileData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
    CPH.SetGlobalVar("R6S_ALPHAPACK_DATA", fileData);
    File.WriteAllText(dataFilePath, fileData);
    return true;
  }
}