using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;


public class Payload<T> {
  [JsonProperty("event")]
  public string Event { get; set; }
  [JsonProperty("data")]
  public T Data { get; set; }
} 
public class VideoPayload {
  [JsonProperty("creator")]
  public string CreatorUserName { get; set; }
  [JsonProperty("broadcaster")]
  public string BroadcasterUserName { get; set; }
  [JsonProperty("broadcasterProfileImageUri")]
  public string BroadcasterProfileImageUri { get; set; }
  [JsonProperty("title")]
  public string Title { get; set; }
  [JsonProperty("videoUri")]
  public string VideoUri { get; set; }
  [JsonProperty("duration")]
  public int Duration { get; set; }
  [JsonProperty("thumbnailUri")]
  public string ThumbnailUri { get; set; }
  [JsonProperty("viewCount")]
  public int ViewCount { get; set; }
}

public class CPHInline {
  public bool Execute() {
    var targetUser = string.Empty;
    var clipCount = 1;

    if (args.ContainsKey("targetUser")) {
      targetUser = args["targetUser"].ToString();
    } else if (args.ContainsKey("input0")) {
      targetUser = args["input0"].ToString();
    }

    if (args.ContainsKey("clipCount")) {
      int.TryParse(args["clipCount"].ToString(), out clipCount);
    }

    if (string.IsNullOrWhiteSpace(targetUser)) {
      return false;
    }

    var allClips = CPH.GetClipsForUser(targetUser);
    if (allClips.Count == 0) {
      return false;
    }

    var randomClips = allClips.OrderBy(c => Guid.NewGuid()).Take(clipCount);
    int count = 0;
    foreach (var clip in randomClips) {
      CPH.SetArgument($"clipBroadcaster{count}", clip.BroadcasterName);
      CPH.SetArgument($"clipTitle{count}", clip.Title);
      CPH.SetArgument($"clipUrl{count}", clip.Url);
      CPH.SetArgument($"clipDuration{count}", clip.Duration);
      CPH.SetArgument($"clipUser{count}", clip.CreatorName);
      CPH.SetArgument($"clipViewCount{count}", clip.ViewCount);
      CPH.SetArgument($"clipThumbnailUrl{count}", clip.ThumbnailUrl);

      var payload = new Payload<VideoPayload> {
        Event = "EVENT_CLIPOVERLAY_PLAY",
        Data = new VideoPayload {
          BroadcasterUserName = clip.BroadcasterName,
          BroadcasterProfileImageUri = GetUserProfileImage(clip.BroadcasterName),
          VideoUri = RegexReplace("(.*)-preview-.*", clip.ThumbnailUrl, "$1.mp4"),
          CreatorUserName = clip.CreatorName,
          ThumbnailUri = clip.ThumbnailUrl,
          Duration = (int)(clip.Duration * 1000),
          Title = clip.Title,
          ViewCount = clip.ViewCount
        }
      };
      CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));
      count++;
    }
    return true;
  }
  private string GetUserProfileImage(string user) {
    try {
      using (var client = new WebClient()) {
        return client.DownloadString($"https://decapi.me/twitch/avatar/{user.ToLower()}").Trim();
      }
    } catch (Exception ex) {
      CPH.LogDebug(ex.ToString());
      return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";
    }
  }
  private string RegexReplace(string pattern, string input, string replacement) {
    Regex re = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
    return re.Replace(input, replacement);
  }
}