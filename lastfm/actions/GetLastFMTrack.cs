using System;
using Newtonsoft.Json;
using System.Net;

public class CPHInline {
  public bool Execute() {
    var lfmUser = "";
    var channel = "";

    if (args.ContainsKey("lastFmUser")) {
      lfmUser = args["lastFmUser"] as string;
    } else {
      CPH.LogDebug("No lastFmUser specified");
      return false;
    }
    if (args.ContainsKey("user")) {
      channel = args["user"] as string;
    } else {
      return false;
    }


    var lastFmDataUrl = $"http://obs-lastfm.herokuapp.com/api/user/tracks/{lfmUser}/?token={channel}";
    try {
      var lastFmData = new WebClient().DownloadString(lastFmDataUrl);
      var lastFmDataJson = JsonConvert.DeserializeObject<LastFmData>(lastFmData);

      if (lastFmDataJson == null) {
        CPH.SetArgument("lastfm_playing", "false");
        CPH.SetArgument("lastfm_title", "");
        CPH.SetArgument("lastfm_artist", "");
        CPH.SetArgument("lastfm_album", "");
        CPH.SetArgument("lastfm_image", "");

        // nothing to show.
        return true;
      }

      CPH.SetArgument("lastfm_playing", "true");
      CPH.SetArgument("lastfm_title", lastFmDataJson.Track);
      CPH.SetArgument("lastfm_artist", lastFmDataJson.Artist);
      CPH.SetArgument("lastfm_album", lastFmDataJson.Album);
      CPH.SetArgument("lastfm_image", lastFmDataJson.Image);

      return true;
    } catch (Exception e) {
      CPH.LogDebug(e.Message);
      return false;
    }
  }
}

public class LastFmData {
  [JsonProperty("title")]
  public string Track { get; set; }
  [JsonProperty("artist")]
  public string Artist { get; set; }
  [JsonProperty("album")]
  public string Album { get; set; }
  [JsonProperty("image")]
  public string Image { get; set; }
}
