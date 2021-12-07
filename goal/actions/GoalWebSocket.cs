using System;
using Newtonsoft.Json;
public class GoalPayload {
  [JsonProperty("event")]
  public string Event { get; set; }
  [JsonProperty("data")]
  public GoalPayloadData Data { get; set; }
}
public class GoalPayloadData {
  [JsonProperty("increase")]
  public decimal Increase { get; set; } = 0M;
  [JsonProperty("increaseFormatted")]
  public string IncreaseFormatted { get { return Increase.ToString("c"); } }

  [JsonProperty("total")]
  public decimal Total { get; set; } = 0M;
  [JsonProperty("totalFormatted")]
  public string TotalFormatted { get { return Total.ToString("c"); } }

  [JsonProperty("goal")]
  public decimal Goal { get; set; } = 0M;
  [JsonProperty("goalFormatted")]
  public string GoalFormatted { get { return Goal.ToString("c"); } }

}
public class CPHInline {
  public bool Execute() {
    decimal increase = 0M;
    decimal total = 0M;
    decimal goal = 0M;
    if (args.ContainsKey("gpbIncrease")) {
      increase = (decimal)args["gpbIncrease"];
    }

    if (args.ContainsKey("gpbTotal")) {
      total = (decimal)args["gpbTotal"];
    }

    if (args.ContainsKey("gpbGoal")) {
      goal = (decimal)args["gpbGoal"];
    }
    var payload = new GoalPayload {
      Event = "EVENT_GOAL_UPDATE",
      Data = new GoalPayloadData {
        Increase = increase,
        Total = total,
        Goal = goal
      }
    };
    CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));
    return true;
  }
}