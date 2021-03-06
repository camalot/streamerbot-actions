# CLIP-PLAYER

This is actions and overlay that will play different types of clips.

- [PlayTwitchClip](#playtwitchclip)
- [PlayRandomTwitchClip](#playrandomtwitchclip)
- [PlayTikTokVideo](#playtiktokvideo)
- [PlayMedalVideo](#playmedalvideo)
- [PlayVideoFile](#playvideofile)


# REQUIREMENTS

This requires the HtmlAgilityPack library to be in the streamer.bot install directory.
Vote up on [this suggestion](https://ideas.streamer.bot/posts/159/nuget-support-for-c-execute-actions) to make this easier.

# ACTIONS

Import into streamer.bot from the [clip-player.action](./clip-player.action) file.

## PlayTwitchClip

This will play the clip that is sent in as `%input0%` or `%twitchVideo%`

### ARGUMENT INPUTS
| NAME | ARGUMENT | DESCRIPTION |  
| --- | :--- | --- |
| Url | <ul><li>`%input0%`</li><li>`%twitchVideo%`</li></ul> | The twitch clip url |  


A regex command that matches the following:

```regex
https:\/\/(?:www|clips)\.twitch\.tv\/(?:[^\/]+\/clip\/)?(?<id>.*?)(?:\?.*)?(?:\s|$)
```

## PlayRandomTwitchClip

This will play `N` number of random clips for a user
### ARGUMENT INPUTS
| NAME | ARGUMENT | DESCRIPTION |  
| --- | :--- | --- |
| TargetUser | <ul><li>`%input0%`</li><li>`%targetUser%`</li></ul> | The target user to play clips for |  

### ARGUMENT OUTPUTS

The number of clips is specified via `%clipCount%`. The default is 1.
It will set the following arguments, where `#` is the index of the clip:

- `clipBroadcaster#`
- `clipTitle#`
- `clipUrl#`
- `clipDuration#`
- `clipUser#`
- `clipViewCount#`
- `clipThumbnailUrl#`

Example command: `!so DarthMinos`


## PlayTikTokVideo

This will play a TikTok video from the tiktok video url

### ARGUMENT INPUTS
| NAME | ARGUMENT | DESCRIPTION |  
| --- | :--- | --- |
| Url | <ul><li>`%input0%`</li><li>`%tiktokVideo%`</li></ul> | The tiktok url to the video |  

A regex command that matches the following:
```regex
https:\/\/(?:www.)?tiktok\.com\/@(.*?)\/video\/(.*?)(\?.*?|$|\s)
```

## PlayMedalVideo

Plays a [medal.tv](https://medal.tv/?ref=DarthMinos_partner) clip.

The Medal desktop client records clips with one button press, posts them on medal.tv, and gives you a shareable link. No lag, no fuss.

| NAME | ARGUMENT | DESCRIPTION |  
| --- | :--- | --- |
| Url | <ul><li>`%input0%`</li><li>`%medalVideo%`</li></ul> | The video file to play |  


A regex command that matches the following:
```regex
https:\/\/(?:www\.)?medal\.tv\/games\/[^\/]+\/clips\/[^\/]+\/[^\/\?]+(?:\?|$)
```

## PlayVideoFile

This will play either a local video file, or a file that is served up by a web server over http/https.

| NAME | ARGUMENT | DESCRIPTION |  
| --- | :--- | --- |
| Url | <ul><li>`%input0%`</li><li>`%video%`</li></ul> | The video file to play. This can be a remote url, or a local file |  


## OBS / SLOBS CONFIGURATION

### ADD THE OVERLAY
Add a browser source and point it to your path of `clip-player/overlay.html`. This should **NOT** be a `local file`. You should use a `file://` path like the following.

![](https://i.imgur.com/D5Dge8f.png)  

Set the width/height to your canvas size, example: 1920x1080, then resize it in OBS to keep the proper scale.