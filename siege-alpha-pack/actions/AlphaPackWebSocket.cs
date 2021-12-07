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
public class Payload {
  [JsonProperty("event")]
  public string Event { get; set; } = "EVENT_R6S_ALPHAPACK";
  [JsonProperty("data")]
  public Dictionary<string, PayloadData> Data { get; set; }

  public Payload() {
    Data = new Dictionary<string, PayloadData>();
    foreach (Rarity rarity in Enum.GetValues(typeof(Rarity))) {
      Data[rarity.ToString().ToLower()] = new PayloadData { Rarity = rarity };
    }
  }
}


[Serializable]
public class PayloadData {
  [JsonProperty("rarity")]
  [JsonConverter(typeof(StringEnumConverter))]
  public Rarity Rarity { get; set; }
  [JsonProperty("count")]
  public int Count { get; set; } = 0;
}
public class CPHInline {
  public bool Execute() {
    var payload = new Payload();
    var data = CPH.GetGlobalVar<string>("R6S_ALPHAPACK_DATA");
    if (string.IsNullOrWhiteSpace(data)) {
      return false;
    }

    var dataObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, PayloadData>>(data);

    if (dataObject != null) {
      payload.Data = dataObject;
    }

    CPH.WebsocketBroadcastJson(Newtonsoft.Json.JsonConvert.SerializeObject(payload));
    return true;
  }
}