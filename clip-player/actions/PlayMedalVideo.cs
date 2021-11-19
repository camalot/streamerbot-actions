// https://medal.tv/games/rainbow-six/clips/4SsIaq-8GMnDG/AEMWvydxFD8D?invite=cr-MSxOdDksNzEwNDI3LA
// https://medal.tv/games/rainbow-six/clips/4SsIaq-8GMnDG/TtHqR2RzBeQH
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
    var medalVideo = string.Empty;

    if (args.ContainsKey("medalVideo")) {
      medalVideo = args["medalVideo"].ToString();
    } else if (args.ContainsKey("input0")) {
      medalVideo = args["input0"].ToString();
    }

    if (string.IsNullOrWhiteSpace(medalVideo)) {
      return false;
    }


    var medal = ParseMedalVideoUrl(medalVideo);
    
    if (medal == null) {
      CPH.LogDebug("Unable to get required medal data");
      return false;
    }

    var payload = new Payload<VideoPayload> {
      Event = "EVENT_CLIPOVERLAY_PLAY",
      Data = new VideoPayload {
        BroadcasterUserName = medal.UserName,
        BroadcasterProfileImageUri = medal.UserProfileImage,
        VideoUri = medal.VideoUrl,
        CreatorUserName = medal.UserName,
        ThumbnailUri = medal.ThumbnailUrl,
        Title = medal.Title,
        Height = medal.Height,
        Width = medal.Width
      }
    };

    if (string.IsNullOrWhiteSpace(medal.VideoUrl)) { return false; }

    CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));

    return true;
  }

  // private string Base64EncodeTiktokVideo(string url, WebClient client) {
  //   client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
  //   client.Headers.Add("accept-language", "en-US,en;q=0.9");
  //   client.Headers.Add("Referer", url);

  //   var data = client.DownloadData(url);
  //   var b64 = Convert.ToBase64String(data);
  //   return $"data:video/mp4;base64,{b64}";
  // }

  private MedalData ParseMedalVideoUrl(string medalVideoUrl) {
    try {
      var regex = new Regex(@"https:\/\/(?:www\.)?medal\.tv\/games\/(?<game>[^\/]+)\/clips\/(?<cid>[^\/]+)\/(?<vid>[^\/\?]+)(?:\?|$)");
      var match = regex.Match(medalVideoUrl);
      if (match.Success) {
        var ttData = new MedalData {
          ContentId = match.Groups["cid"].Value,
          VideoId = match.Groups["vid"].Value,
          Game = match.Groups["game"].Value,
        };
        CPH.LogDebug($"User: {ttData.UserName}");
        using (var client = new WebClient()) {
          CPH.LogDebug($"https://medal.tv/games/{ttData.Game}/clips/{ttData.ContentId}/{ttData.VideoId}");
          client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");

          client.Headers.Add("Referer", medalVideoUrl);
          var data = client.DownloadData($"https://medal.tv/games/{ttData.Game}/clips/{ttData.ContentId}/{ttData.VideoId}");
          var html = Encoding.UTF8.GetString(data);
          var doc = new HtmlDocument();

          // var jsRegex = new Regex(@"var hydrationData\s?=\s?(\{.*\};?)\s*<\/script>", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
          // var jsMatch = jsRegex.Match(html);
          // if (jsMatch.Success) {
          //   var js = jsMatch.Groups[1].Value;
          //   js = js.Replace($"\"{ttData.ContentId}\":{{", "content");
          //   var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(js);
          //   ttData.Title = json.clips.content.contentTitle;
          //   ttData.UserName = json.clips.content.poster.userName;
          //   ttData.UserProfileImage = json.clips.content.poster.thumbnail;
          // }

          var userInfoRegex = new Regex(@"\""displayName\"":\s*\""(?<userName>[^\""]+)\"",\s*\""thumbnail\"":\s*\""(?<profileImage>https:\/\/ cdn\.medal\.tv\/ avatars\/[^\/] +\/[^\.] +\.png)\""");
          var matches = userInfoRegex.Matches(html);
          if (matches.Count > 0) {
            Match lastMatch = matches[matches.Count - 1];
            ttData.UserName = lastMatch.Groups["userName"].Value;
            ttData.UserProfileImage = lastMatch.Groups["profileImage"].Value;
            CPH.LogDebug($"userName: {ttData.UserName}");
            CPH.LogDebug($"profileImage: {ttData.UserProfileImage}");
          }

          doc.LoadHtml(html);
          if (string.IsNullOrEmpty(ttData.Title)) {
            var titleNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']");
            if (titleNode != null) {
              ttData.Title = titleNode.GetAttributeValue("content", "").Replace("recorded with Medal.tv", "").Trim();
              CPH.LogDebug($"title: {ttData.Title}");
            } else {
              CPH.LogDebug("No title found");
            }
          }
          var videoNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:video:secure_url']");
          if (videoNode != null) {
            var ttVideoUrl = videoNode.GetAttributeValue("content", "").Replace("&amp;", "&");
            CPH.LogDebug(ttVideoUrl);
            ttData.VideoUrl = ttVideoUrl;
            //ttData.VideoUrl = Base64EncodeTiktokVideo(ttVideoUrl, client);
          } else {
              CPH.LogDebug("No video found");
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
          // var hNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:video:height']");
          // int h = 0;
          // if (hNode != null) {
          //   int.TryParse(hNode.GetAttributeValue("content", "0"), out h);
          // }
        
          // ttData.Height = h;
          // var wNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:video:width']");
          // int w = 0;
          // if (wNode != null) {
          //   int.TryParse(wNode.GetAttributeValue("content", "0"), out w);
          // }          
          // ttData.Width = w;
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

public class MedalData
{
  public string ContentId { get; set; }
  public string UserName { get; set; }
  public string VideoId { get; set; }
  public string VideoType { get; set; }
  public string VideoUrl { get; set; }
  public string Game { get; set; }
  public string Title { get; set; }
  public string VideoTitle { get; set; }
  public string ThumbnailUrl { get; set; }
  public string UserProfileImage { get; set; }
  public int Height { get; set; }
  public int Width { get; set; }
}
