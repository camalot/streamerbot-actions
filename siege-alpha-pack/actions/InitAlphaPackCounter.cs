using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
public enum Rarity {
  Alpha,
  Bravo,
  Special,
  Common,
  Uncommon,
  Rare,
  Epic,
  Legendary,
  Duplicate
}


[Serializable]
public class PayloadData {
  [JsonProperty("rarity")]
  [JsonConverter(typeof(StringEnumConverter))]
  public Rarity Rarity { get; set; }
  [JsonProperty("count")]
  public int Count { get; set; } = 0;
}


public class InitData : Dictionary<string, PayloadData> {
  public InitData() {
    foreach (Rarity rarity in Enum.GetValues(typeof(Rarity))) {
      this[rarity.ToString().ToLower()] = new PayloadData { Rarity = rarity };
    }
  }
}
public class CPHInline {
  public bool Execute() {
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