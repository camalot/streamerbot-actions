// https://www.twitch.tv/darthminos/clip/ConsiderateElegantOcelotDeIlluminati-Feq2up8f1BuTK8SG
// https://clips.twitch.tv/FlaccidConsiderateLobsterRuleFive-T4HiWKBupEyOeEja
// https:\/\/(?:www|clips)\.twitch\.tv\/(?:[^\/]+\/clip\/)?(?<id>.*?)(?:\?.*)?(?:\s|$)

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Net;
using System.Text;

public class Payload<T>
{
    [JsonProperty("event")]
    public string Event { get; set; }
    [JsonProperty("data")]
    public T Data { get; set; }
}
public class VideoPayload
{
    [JsonProperty("videoUri")]
    public string VideoUri { get; set; }
}

public class CPHInline
{
    public bool Execute()
    {
        var inputVideo = string.Empty;

        if (args.ContainsKey("twitchVideo"))
        {
            inputVideo = args["twitchVideo"].ToString();
        }
        else if (args.ContainsKey("input0"))
        {
            inputVideo = args["input0"].ToString();
        }


        if (string.IsNullOrWhiteSpace(inputVideo))
        {
            return false;
        }

        var clipVideoUrl = GetVideoUrlFromClipr(inputVideo);

        if(string.IsNullOrWhiteSpace(clipVideoUrl)) {
            return false;
        }

        var payload = new Payload<VideoPayload>
        {
            Event = "EVENT_CLIPOVERLAY_PLAY",
            Data = new VideoPayload {
                VideoUri = clipVideoUrl,
            }
        };
        CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));
        return true;
    }

    private string GetVideoUrlFromClipr(string twitchVideoUrl)
    {
        var twitchIdRegex = new Regex(@"https:\/\/(?:www|clips)\.twitch\.tv\/(?:[^\/]+\/clip\/)?(?<id>.*?)(?:\?.*)?$");
        var match = twitchIdRegex.Match(twitchVideoUrl);
        if (!match.Success)
        {
            return null;
        }
        var clipId = match.Groups["id"].Value;
        var cliprUrl = $"https://clipr.xyz/{clipId}";
        var html = GetWebContent(cliprUrl);
        var regex = new Regex(@"href=""(?<url>https:\/\/.*?\.twitch\.tv\/.*?\.mp4)""");
        match = regex.Match(html);
        if (!match.Success)
        {
            return null;
        }
        var cliprVideoUrl = match.Groups["url"].Value;
        return cliprVideoUrl;
    }

    private string GetWebContent(string url) {
        using (var client = new WebClient())
        {
            client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36");
            client.Headers.Add("Referer", url);
            var data = client.DownloadData(url);
            var html = Encoding.UTF8.GetString(data);
            return html;
        }
    }
}