using System;
using Newtonsoft.Json;
public class Payload {
  [JsonProperty("event")]
  public string Event { get; set; }
  [JsonProperty("data")]
  public PayloadData Data { get; set; }
}

public class PayloadSettings {
  [JsonProperty("Duration")]
  public int Duration { get; set; } = 10;
  public string FontName { get; set; } = "Bakbak One";
  public bool ShowLink { get; set; } = true;
  public string LinkText { get; set; } = "twitch.tv/{username}";
  public string OutTransition { get; set; } = "slideOutLeft";
  public string InTransition { get; set; } = "slideInRight";
  public string AttentionAnimation { get; set; } = "none";
  public bool EnableShadow { get; set; } = true;
  public string UserColor { get; set; } = "rgba(255,255,255,1)";
  public string LinkColor { get; set; } = "rgba(255,0,0,1)";
  public string ImageShape { get; set; } = "circle";
}

public class PayloadData {
  [JsonProperty("user")]
  public string User { get; set; }
  [JsonProperty("settings")]
  public PayloadSettings Settings { get; set; }
}

public class CPHInline {
  public bool Execute() {
    var username = "";

    if (args.ContainsKey("input0")) {
      username = args["input0"] as string;
    } else if (args.ContainsKey("targetUser")) {
      username = args["targetUser"] as string;
    } else if (args.ContainsKey("user")) {
      username = args["user"] as string;
    } else {
      return false;
    }

    var settings = new PayloadSettings();
    if (args.ContainsKey("duration")) {
      var val = settings.Duration;
      if (int.TryParse(args["duration"].ToString(), out val)) {
        settings.Duration = val;
      }
    }
    if (args.ContainsKey("outTransition")) {
      settings.OutTransition = args["outTransition"] as string;
    }
    if (args.ContainsKey("inTransition")) {
      settings.InTransition = args["inTransition"] as string;
    }
    if (args.ContainsKey("attentionAnimation")) {
      settings.AttentionAnimation = args["attentionAnimation"] as string;
    }
    if (args.ContainsKey("enableShadow")) {
      var esVal = settings.EnableShadow;
      if (bool.TryParse(args["enableShadow"].ToString(), out esVal)) {
        settings.EnableShadow = esVal;
      }
    }
    if (args.ContainsKey("userColor")) {
      settings.UserColor = args["userColor"] as string;
    }
    if (args.ContainsKey("linkColor")) {
      settings.LinkColor = args["linkColor"] as string;
    }
    if (args.ContainsKey("imageShape")) {
      settings.ImageShape = args["imageShape"] as string;
    }
    if (args.ContainsKey("fontName")) {
      settings.FontName = args["fontName"] as string;
    }
    if (args.ContainsKey("showLink")) {
      var slVal = settings.ShowLink;
      if (bool.TryParse(args["showLink"].ToString(), out slVal)) {
        settings.ShowLink = slVal;
      }
    }
    if (args.ContainsKey("linkText")) {
      settings.LinkText = args["linkText"] as string;
    }

    var payload = new Payload {
      Event = "EVENT_SO_COMMAND",
      Data = new PayloadData {
        User = username,
        Settings = settings
      }
    };
    CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));
    return true;
  }
}
