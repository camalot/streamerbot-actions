# CLIP-PLAYER

This is actions and overlay that will play different types of clips.

- [PlayTwitchClip](#playtwitchclip)
- [PlayRandomTwitchClip](#playrandomtwitchclip)
- [PlayTikTokVideo](#playtiktokvideo)
- [PlayVideoFile](#playvideofile)


# REQUIREMENTS

This requires the HtmlAgilityPack library to be in the streamer.bot install directory.
Vote up on [this suggestion](https://ideas.streamer.bot/posts/159/nuget-support-for-c-execute-actions) to make this easier.

# ACTIONS

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

## PlayVideoFile

This will play either a local video file, or a file that is served up by a web server over http/https.

| NAME | ARGUMENT | DESCRIPTION |  
| --- | :--- | --- |
| Url | <ul><li>`%input0%`</li><li>`%video%`</li></ul> | The video file to play. This can be a remote url, or a local file |  
