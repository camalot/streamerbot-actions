(function () {
  "use strict";

  const settings = { ...window.DEFAULT_SETTINGS, ...window.settings };
  function loadFontsScript(font) {
    WebFont.load({
      active: function () {
        console.log(`all fonts loaded`);
        $(":root").css("--font-family", `${font}, ${settings.FallBackFonts.join(",")}, Arial, sans-serif`);
      },
      fontactive: function (familyName, fvd) {  console.log(`loaded ${familyName}`); },
      fontloading: function (familyName, fvd) { console.log(`loading ${familyName}`); },
      google: {
        families: [...[font], ...settings.FallBackFonts]
      }
    });
  }


  function initializeUI() {
    $(":root")
      .css("--font-size", settings.FontSize)
      .css("--font-color", settings.FontColor)
      ;
    loadFontsScript(settings.FontName);

  }

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
            "EVENT_R6S_ALPHAPACK"
          ]
        },
        "id": "r6sap-overlay"
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
        let initSocket = new WebSocket(settings.SB_CustomWebSocket);
        initSocket.onopen = function () { console.log("init open"); };
        initSocket.onmessage = function (message) { console.log(`init message: ${message}`); };
        return;
      }
      // parse socketMessage.data, if it is a string.
      let eventData = typeof socketMessage.data === "string" ? JSON.parse(socketMessage.data || "{}") : socketMessage.data;
      if (!eventData) {
        return;
      }

      let payload = eventData;
      if (payload && payload.data && payload.event) {
        let eventName = payload.event;
        switch (eventName) {
          case "EVENT_R6S_ALPHAPACK":
            console.log("EVENT_R6S_ALPHAPACK");
            console.log(payload.data);
            /*
              {
                  "event": "EVENT_R6S_ALPHAPACK",
                  "data": {
                    "common": {
                      "rarity": "Common",
                      "count": 1
                    },
                    "uncommon": {
                      "rarity": "Uncommon",
                      "count": 1
                    },
                    "rare":{
                      "rarity": "Rare",
                      "count": 1
                    },
                    "epic": {
                      "rarity": "Epic",
                      "count": 1
                    },
                    "legendary": {
                      "rarity": "Legendary",
                      "count": 1
                    },
                    "duplicate": {
                      "rarity": "Duplicate",
                      "count": 1
                    },
                  }
              }
            */
            let d = payload.data;
            for (let rarity in d) {
              let count = d[rarity].count;
              $(`#${rarity}-count`).text(count);
            }
            break;
          default:
            console.log(eventName);
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


  jQuery(document).ready(function () {
    initializeUI();
    connectWebsocket();
  });
})();