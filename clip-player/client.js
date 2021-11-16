(function () {
  "use strict";
  const IS_CEF = window.obsstudio !== undefined;
  let isClipPlaying = false;
  let queueInitialized = false;
  let queueWatcher = null;
  let clipQueue = [];
  let blank = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";
  const settings = { ...window.DEFAULT_SETTINGS, ...window.settings };
  function loadFonts(font) {
    WebFont.load({
      active: function () {
        console.log(`all fonts loaded`);
        $(":root").css("--font-family", `${font}, ${settings.FallBackFonts.join(",")}, Arial, sans-serif`);
      },
      fontactive: function (familyName, fvd) { console.log(`loaded ${familyName}`); },
      fontloading: function (familyName, fvd) { console.log(`loading ${familyName}`); },
      google: {
        families: [...[font], ...settings.FallBackFonts]
      }
    });
  }


  function initializeUI() {
    loadFonts(settings.FontName);
    $(":root")
      .css("--watermark", `url(${blank})`);


    $("#video-container .video-box video.video-player")
      .on("error", function (e) {
        console.error(`Error: ${e}`);
        videoEnded(e);
      })
      .prop("volume", settings.Volume / 100)
      .prop("muted", IS_CEF ? settings.MuteAudio : true)
      .on("canplay", videoLoaded)
      .on("ended pause", videoEnded)
      .on("timeupdate", timelapse);

    if (settings.ProgressBarBackgroundColor != null && settings.ProgressBarBackgroundColor !== "") {
      $(":root")
        .css("--progress-bg", settings.ProgressBarBackgroundColor);
    }

    if (settings.ProgressBarFillColor != null && settings.ProgressBarFillColor !== "") {
      $(":root")
        .css("--progress-fill", settings.ProgressBarFillColor);
    }
  }




  let timelapse = (e) => {

    let video = $("#video-container .video-box video.video-player").get(0);
    let pbar = $("#video-container .video-box progress").get(0);
    if (video.duration && video.currentTime) {
      let percent = (100 / video.duration) * video.currentTime;
      pbar.value = percent;
    } else {
      pbar.value = 0;
    }
  };

  let videoLoaded = (e) => {
    isClipPlaying = true;
    $("#video-container progress").removeClass("hidden");
    $("#video-container .video-box .watermark").removeClass("hidden");

  };

  let videoEnded = (e) => {
    isClipPlaying = false;
    if (clipQueue.length > 0) {
      return queueVideo(clipQueue.shift());
    } else {
      queueInitialized = false;
      watchClipQueue();
      console.log(`EVENT_CLIPOVERLAY_PLAY: no more clips`);
      // done playing all clips
      $("#video-container .video-box video.video-player")
        .empty()
        .addClass("hidden");
      $(":root")
        .css("--watermark", `url(${blank})`);
      $("#video-container progress").addClass("hidden").val(0);
      $("#video-container .video-box .watermark").addClass("hidden");
      $("#video-container .video-box .title").addClass("hidden");
      $("#video-container .video-box .views").addClass("hidden");
    }
  };

  let queueVideo = (clipData) => {
    if (clipData) {
      isClipPlaying = true;
      $("#video-container .video-box video.video-player")
        .prop("autoplay", true)
        .prop("preload", true)
        .prop("onend", "")
        .prop("loop", false)
        .prop("volume", settings.Volume / 100)
        .attr("src", clipData.videoUri)
        .empty()
        .removeClass("hidden")
        .append(`<source src="${clipData.videoUri}" type="video/mp4" />`);

      let videoPlayer = $("#video-container .video-box video.video-player").get(0);
      videoPlayer.playbackRate = settings.PlaybackSpeed || 1.0;
      console.log(`playback speed: ${settings.PlaybackSpeed || 1.0}`);

      console.log(`SETTING WATERMARK: ${clipData.broadcasterProfileImageUri}`);
      $(":root")
        .css("--watermark", `url(${clipData.broadcasterProfileImageUri || blank})`);

      if (clipData.height > 0 && clipData.width > 0) {
        console.log(`setting video height/width: ${clipData.height}px/${clipData.width}px`);
        $(":root")
          .css("--video-height", `${clipData.height}px`)
          .css("--video-width", `${clipData.width}px`);
      } else {
        console.log(`setting video height/width: 100%/100%`);
        $(":root")
          .css("--video-height", "100%")
          .css("--video-width", "100%");
      }

      let title = $("#video-container .video-box .title");
      title.html(clipData.title);
      if (settings.ShowTitle) {
        title.removeClass("hidden");
      } else {
        title.addClass("hidden");
      }
      $("#video-container .video-box .views")
        .html(clipData.viewCount);
    }
  };



  function connectWebsocket() {
    //-------------------------------------------
    //  Create WebSocket
    //-------------------------------------------
    let socket = new WebSocket(settings.SB_WebSocket);

    //-------------------------------------------
    //  Websocket Event: OnOpen
    //-------------------------------------------
    socket.onopen = function () {
      const auth = {
        "author": "DarthMinos",
        "website": "https://perks.darthminos.tv",
        "request": "Subscribe",
        "events": {
          "general": ["custom"],
          "websocketClient": ["open", "message"],
          "custom": [
            "EVENT_CLIPOVERLAY_PLAY",
            "EVENT_CLIPOVERLAY_STOP"
          ]
        },
        "id": "clip-player"
      };
      socket.send(JSON.stringify(auth));
    };

    //-------------------------------------------
    //  Websocket Event: OnMessage
    //-------------------------------------------
    socket.onmessage = function (message) {
      console.log(message);
      // Parse message
      let socketMessage = JSON.parse(message.data);
      // this connects to SB custom websocket to trigger the init
      if (socketMessage.status === "ok") {
        console.log("init");
        // let initSocket = new WebSocket(settings.SB_CustomWebSocket);
        // initSocket.onopen = function () { console.log("init open"); };
        // initSocket.onmessage = function (message) { console.log(`init message: ${message}`); };
        return;
      }
      // parse socketMessage.data, if it is a string.
      let eventData = typeof socketMessage.data === "string" ? JSON.parse(socketMessage.data || "{}") : socketMessage.data;
      if (!eventData) {
        return;
      }

      let payload = eventData;
      if (payload && payload.data && payload.event) {
        let payloadEvent = payload.event;
        let payloadData = payload.data;

        let videoPlayer = $("#video-container video.video-player").get(0);
        switch (payloadEvent) {
          case "EVENT_CLIPOVERLAY_STOP":
            break;
          case "EVENT_CLIPOVERLAY_PLAY":
            /*
              {
                  "event": "EVENT_CLIPOVERLAY_PLAY",
                  "data": {
                      "broadcaster": "GuyNameMike",
                      "broadcasterProfileImageUri": null,
                      "creator": "GuyNameMike",
                      "duration": 30000,
                      "thumbnailUri": "https://clips-media-assets2.twitch.tv/42448507196-offset-29014-preview-480x272.jpg",
                      "title": "Will Menna Kidnap BnD Tomorrow? - !dixper #FishingNorthAtlantic",
                      "videoUri": "https://clips-media-assets2.twitch.tv/42448507196-offset-29014.mp4",
                      "viewCount": 1
                  }
              }
            */
            console.log(`EVENT_CLIPOVERLAY_PLAY: queueing: ${payloadData.title}`);
            clipQueue.push(payloadData);


            break;
          default:
            console.log(payloadEvent);
            break;
        }
      }
    };

    //-------------------------------------------
    //  Websocket Event: OnError
    //-------------------------------------------
    socket.onerror = function (error) {
      console.error(`Error: ${error}`);
    };

    //-------------------------------------------
    //  Websocket Event: OnClose
    //-------------------------------------------
    socket.onclose = function () {
      console.log("close");
      // Clear socket to avoid multiple ws objects and EventHandlings
      socket = null;
      // Try to reconnect every 5s
      setTimeout(function () { connectWebsocket(); }, 5000);
    };

  }

  function watchClipQueue() {
    queueWatcher = setInterval(function () {
      if (clipQueue.length > 0 && !queueInitialized) {
        queueInitialized = true;
        queueVideo(clipQueue.shift());
        clearInterval(queueWatcher);
      }
    }, 500);
  }


  jQuery(document).ready(function () {
    initializeUI();
    $("video.video-player").trigger("click");
    connectWebsocket();
    watchClipQueue();
    
  });

})();