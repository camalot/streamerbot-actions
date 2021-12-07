
using System;
using Newtonsoft.Json;
using System.Text;

public class Payload<T> {
  [JsonProperty("event")]
  public string Event { get; set; }
  [JsonProperty("data")]
  public T Data { get; set; }
}
public class VideoPayload {
  [JsonProperty("videoUri")]
  public string VideoUri { get; set; }
} 

public class CPHInline {
  public bool Execute() {
    var inputVideo = string.Empty;

    if (args.ContainsKey("video")) {
      inputVideo = args["video"].ToString();
    } else if (args.ContainsKey("input0")) {
      inputVideo = args["input0"].ToString();
    }

    if (System.IO.Path.IsPathRooted(inputVideo)) {
      inputVideo = inputVideo.Replace(@"\", "/");
    }

    if (!IsRemoteVideo(inputVideo) && !IsVideoPathUriSet(inputVideo)) {
      inputVideo = $"file://{inputVideo}";
    }


    if (string.IsNullOrWhiteSpace(inputVideo)) {
      return false;
    }

    var payload = new Payload<VideoPayload> {
      Event = "EVENT_CLIPOVERLAY_PLAY",
      Data = new VideoPayload {
        VideoUri = inputVideo,
      }
    };
    CPH.WebsocketBroadcastJson(JsonConvert.SerializeObject(payload));
    return true;
  }

  private bool IsVideoPathUriSet(string inputVideo) {
    return inputVideo.StartsWith("file://");
  }

  private bool IsRemoteVideo(string inputVideo) {
    return inputVideo.StartsWith("http://") || inputVideo.StartsWith("https://");
  }
}