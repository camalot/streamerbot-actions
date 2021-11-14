(function () {
  "use strict";

  const settings = { ...window.DEFAULT_SETTINGS, ...window.settings };
  function loadFontsScript(font) {
    // let fnt = font.toLowerCase().replace(" ", "-");
    // var script = document.createElement("script");
    // script.onload = function () {
    //   $(":root").css("--font-name", `${fnt}, Arial, sans-serif`);
    // };
    // script.src = `http://use.edgefonts.net/${fnt}.js`;

    // document.head.appendChild(script);

    WebFont.load({
      active: function () {
        console.log(`all fonts loaded`);
        $(":root").css("--pb-font-family", `${font}, ${settings.FallBackFonts.join(",")}, Arial, sans-serif`);
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
      .css("--pb-fill-width", "0%")
      ;
    $(".label").html(settings.Label);
    loadFontsScript(settings.FontName);

  }



  function increaseAnimation(start, end, final) {
    const duration = 1000;
    const frameRate = 1000 / 60;
    const frames = Math.round(duration / frameRate);
    const easeOutQuad = t => t * (2 - t);
    const animateCountUp = el => {
      let frame = 0;
      const countTo = end;
      const countFrom = start;
      // Start the animation running 60 times per second
      const counter = setInterval(() => {
        frame++;
        // Calculate our progress as a value between 0 and 1
        // Pass that value to our easing function to get our
        // progress on a curve
        const progress = easeOutQuad(frame / frames);
        // Use the progress value to calculate the current count
        const currentCount = Math.round((countTo - countFrom) * progress);

        // If the current count has changed, update the element
        if (countFrom !== currentCount) {
          let val = `${currentCount + countFrom}`;
          let splitVal = val.split(".");
          if (splitVal.length === 1) {
            el.innerHTML = `$${splitVal[0]}.00`;
          } else {
            if (splitVal[1].length === 1) {
              el.innerHTML = `$${splitVal[0]}.${splitVal[1]}0`;
            } else {
              el.innerHTML = `$${splitVal[0]}.${splitVal[1]}`;
            }
          }
        }

        // If we’ve reached our last frame, stop the animation
        if (frame === frames) {
          clearInterval(counter);
          el.innerHTML = final;
        }
      }, frameRate);
    };

    const countupEls = document.querySelectorAll(".current");
    countupEls.forEach(animateCountUp);
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
            "EVENT_GOAL_UPDATE"
          ]
        },
        "id": "gpb-overlay"
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
          case "EVENT_GOAL_UPDATE":
            /*
              {
                  "event": "EVENT_GOAL_UPDATE",
                  "data": {
                      "increase": 0.17,"
                      "increaseFormatted": "$0.17",
                      "total": 135.67,
                      "totalFormatted": "$135.67",
                      "goal": 800,
                      "goalFormatted": "$800.00"
                  }
              }
            */
            let d = payload.data;
            let percent = (d.total / d.goal) * 100;
            $(":root").css("--pb-fill-width", `${percent}%`);

            increaseAnimation(parseFloat($(".current").html().replace("$", ""), 2), d.total, d.totalFormatted);

            $(".goal").html(`$${d.goal}`);
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