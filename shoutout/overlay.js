"use strict";
const settings = {
	...window.DEFAULT_SETTINGS,
	...window.settings
};

function loadFontsScript(font, fallbackFonts) {

	WebFont.load({
		active: function () {
			console.log(`all fonts loaded`);
			$(":root").css("--font-family", `${font}, ${fallbackFonts.join(",")}, Arial, sans-serif`);
		},
		fontactive: function (familyName, fvd) {
			console.log(`loaded ${familyName}`);
		},
		fontloading: function (familyName, fvd) {
			console.log(`loading ${familyName}`);
		},
		google: {
			families: [...[font], ...fallbackFonts]
		}
	});
}

let animationEndClasses = "webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend";

function showCaster(data) {
	if (!data || !data.user || !data.image) {
		return;
	}
	console.log(data);
	$("#alert")
		.queue(function () {
			$("#name").html(data.user.toUpperCase());
			$("#logo img").attr("src", data.image);
			$("#link").html(data.settings.LinkText.replace("\{username\}", data.user.toLowerCase()));

			$("#alert")
				.removeClass()
				.addClass(`${data.settings.InTransition} animated`)
				.one(animationEndClasses, function () {
					$(this)
						.off(animationEndClasses)
						.removeClass()
						.addClass(`${data.settings.AttentionAnimation} animated`)
						.one(animationEndClasses, function () {
							$(this)
								.removeClass();
						});
				})
				.dequeue();
		})
		.delay((data.settings.Duration || 10) * 1000)
		.queue(function () {
			$("#alert")
				.removeClass()
				.off(animationEndClasses)
				.addClass(`${data.settings.AttentionAnimation} animated`)
				.one(animationEndClasses, function () {
					$(this)
						.removeClass()
						.addClass(`${data.settings.OutTransition} animated`)
						.one(animationEndClasses, function () {
							$(this)
								.removeClass().addClass("hidden");
						});
				})
				.dequeue();
		});
}

function initializeUI(s) {
	loadFontsScript(s.FontName, s.FallBackFonts);

	$(":root")
		.css("--link-color", `${s.LinkColor || "rgba(230,126,34,1)"}`)
		.css("--name-color", `${s.UserColor || "rgba(255, 0, 0, 1)"}`)
		.css("--link-visible", s.ShowLink ? "inline-block" : "none");

	$("#logo img").removeClass().addClass(`${s.ImageShape} ${s.EnableShadow ? "shadow" : ""}`);
	$("#name, #link").removeClass().addClass(`${s.EnableShadow ? "shadow" : ""}`);
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
					"EVENT_SO_COMMAND"
				]
			},
			"id": "so-overlay"
		};
		socket.send(JSON.stringify(auth));
	};

	//-------------------------------------------
	//  Websocket Event: OnMessage
	//-------------------------------------------
	socket.onmessage = function (message) {
		// console.log(message);
		// Parse message
		let socketMessage = JSON.parse(message.data);
		// this connects to SB custom websocket to trigger the init
		if (socketMessage.status === "ok") {
			console.log("init");
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
				case "EVENT_SO_COMMAND":
					console.log("shout out event");
					let d = payload.data;
					console.log(d);
					if (d.settings) {
						initializeUI({
							...window.DEFAULT_SETTINGS,
							...d.settings
						});
					}
					$.ajax({
						type: 'GET',
						url: 'https://decapi.me/twitch/avatar/' + d.user,
						success: function (data) {
							if (data) {
								showCaster({
									user: d.user,
									image: data,
									settings: {
										...window.DEFAULT_SETTINGS,
										...d.settings
									}
								});
							}
						},
						error: function (err) {
							console.error(err);
						}
					});
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
		setTimeout(function () {
			connectWebsocket();
		}, 5000);
	};

}


jQuery(document).ready(function () {
	connectWebsocket();
});