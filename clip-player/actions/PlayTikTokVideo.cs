// https://www.tiktok.com/@mr.smoothhimself/video/7025537687411281199?sender_device=pc&sender_web_id=6946786531576022534&is_from_webapp=v1&is_copy_url=0
// <span class="tiktok-avatar tiktok-avatar-circle avatar live-image-animation jsx-3659161049" style="cursor: pointer; width: 56px; height: 56px;"><img alt="" src="https://p16-sign-va.tiktokcdn.com/tos-maliva-avt-0068/c050169f2775190e4cd410be419cabb7~c5_100x100.jpeg?x-expires=1637121600&amp;x-signature=q690Ei4J2f%2F85VPY%2FaD2JwNm3Vw%3D"></span>

// @nuget: HtmlAgilityPack
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using HtmlAgilityPack;
using System.Xml;
using System.Text;
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
  [JsonProperty("thumbnailUri")]
  public string ThumbnailUri { get; set; }
  [JsonProperty("height")]
  public int Height { get; set; }
  [JsonProperty("width")]
  public int Width { get; set; }
}

public class CPHInline {
  public bool Execute() {
    var tiktokVideo = string.Empty;

    if (args.ContainsKey("tiktokVideo")) {
      tiktokVideo = args["tiktokVideo"].ToString();
    } else if (args.ContainsKey("input0")) {
      tiktokVideo = args["input0"].ToString();
    }

    if (string.IsNullOrWhiteSpace(tiktokVideo)) {
      return false;
    }


    var tiktok = ParseTiktokVideoUrl(tiktokVideo);

    if (tiktok == null) {
      CPH.LogDebug("Unable to get required tiktok data");
      return false;
    }

    var payload = new Payload<VideoPayload> {
      Event = "EVENT_CLIPOVERLAY_PLAY",
      Data = new VideoPayload {
        BroadcasterUserName = tiktok.UserName,
        BroadcasterProfileImageUri = tiktok.UserProfileImage,
        VideoUri = tiktok.VideoUrl,
        CreatorUserName = tiktok.UserName,
        ThumbnailUri = tiktok.ThumbnailUrl,
        Title = tiktok.Title,
        Height = tiktok.Height,
        Width = tiktok.Width
      }
    };

    if (string.IsNullOrWhiteSpace(tiktok.VideoUrl)) { return false; }

    CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));

    return true;
  }

  private string FindTiktokVideoUrl(string html) {
    var match = Regex.Match(html, @"<meta property=""og:video:secure_url"" content=""(?<url>[^""]*)""", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
    if (match.Success) {
      return match.Groups["url"].Value.Replace("&amp;", "&");
    }
    return string.Empty;
  }

  private string FindTiktokUserProfileImage(string html) {
    var match = Regex.Match(html, @"<span\sclass=['""]tiktok-avatar\s[^'""]+.*?>.*?<img.*?src=['""](?<url>[^'""]*)['""]", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
    if (match.Success) {
      return match.Groups["url"].Value.Replace("&amp;", "&");
    }
    CPH.LogDebug("MATCH FAIL: Could not find user profile image");
    return string.Empty;
  }

  private string Base64EncodeTiktokVideo(string url, WebClient client) {
    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
    client.Headers.Add("accept-language", "en-US,en;q=0.9");
    client.Headers.Add("Referer", url);

    var data = client.DownloadData(url);
    var b64 = Convert.ToBase64String(data);
    return $"data:video/mp4;base64,{b64}";
  }

  private TiktokData ParseTiktokVideoUrl(string tiktokVideo) {
    try {
      var regex = new Regex(@"https:\/\/(?:www.)?tiktok\.com\/@(.*?)\/video\/(.*?)(\?.*?|$|\s)");
      var match = regex.Match(tiktokVideo);
      if (match.Success) {
        var ttData = new TiktokData {
          UserName = match.Groups[1].Value,
          VideoId = match.Groups[2].Value
        };
        CPH.LogDebug($"User: {ttData.UserName}");
        using (var client = new WebClient()) {
          CPH.LogDebug($"https://www.tiktok.com/@{ttData.UserName}/video/{ttData.VideoId}");
          client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");

          client.Headers.Add("Referer", tiktokVideo);
          var data = client.DownloadData($"https://www.tiktok.com/@{ttData.UserName}/video/{ttData.VideoId}");
          var html = Encoding.UTF8.GetString(data);
          var doc = new HtmlDocument();

          doc.LoadHtml(html);
          var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");
          if (titleNode != null) {
            ttData.Title = titleNode.GetAttributeValue("content", "");
            CPH.LogDebug($"title: {ttData.Title}");
          } else {
            CPH.LogDebug("No title found");
          }
          var videoNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:video:secure_url']");
          if (videoNode != null) {
            var ttVideoUrl = videoNode.GetAttributeValue("content", "").Replace("&amp;", "&");
            ttData.VideoUrl = Base64EncodeTiktokVideo(ttVideoUrl, client);
          } else {
            var dlUrl = FindTiktokVideoUrl(html);
            if (!string.IsNullOrWhiteSpace(dlUrl)) {
              ttData.VideoUrl = Base64EncodeTiktokVideo(dlUrl, client);
            } else {
              CPH.LogDebug("No video found");
            }
          }
          var thumbNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
          if (thumbNode != null) {
            ttData.ThumbnailUrl = thumbNode.GetAttributeValue("content", "").Replace("&amp;", "&");
            CPH.LogDebug($"thumbnail: {ttData.ThumbnailUrl}");
          } else {
            CPH.LogDebug("No thumbnail found");
          }

          var videoTypeNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:video:type']");
          if (videoTypeNode != null) {
            ttData.VideoType = videoTypeNode.GetAttributeValue("content", "");
            CPH.LogDebug($"video type: {ttData.VideoType}");
          } else {
            CPH.LogDebug("No video type found");
          }
          var hNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:video:height']");
          int h = 0;
          if (hNode != null) {
            int.TryParse(hNode.GetAttributeValue("content", "0"), out h);
          }

          ttData.Height = h;
          var wNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:video:width']");
          int w = 0;
          if (wNode != null) {
            int.TryParse(wNode.GetAttributeValue("content", "0"), out w);
          }
          ttData.Width = w;


          CPH.LogDebug($"H/W: {h}px/{w}px");
          CPH.LogDebug("Getting Profile Image");
          var profileImageNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'tiktok-avatar')]/img");
          if (profileImageNode != null) {
            ttData.UserProfileImage = profileImageNode.GetAttributeValue("src", "").Replace("&amp;", "&");
            CPH.LogDebug($"profileImage: {ttData.UserProfileImage}");
          } else {
            ttData.UserProfileImage = FindTiktokUserProfileImage(html);
            CPH.LogDebug($"profileImage: {ttData.UserProfileImage}");
          }
          return ttData;
        }
      }
      return null;
    } catch (Exception ex) {
      CPH.LogDebug(ex.ToString());
      return null;
    }
  }
}

public class TiktokData {
  public string UserName { get; set; }
  public string VideoId { get; set; }
  public string VideoType { get; set; }
  public string VideoUrl { get; set; }
  public string Title { get; set; }
  public string VideoTitle { get; set; }
  public string ThumbnailUrl { get; set; }
  public string UserProfileImage { get; set; }
  public int Height { get; set; }
  public int Width { get; set; }
}
