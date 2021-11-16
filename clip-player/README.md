# CLIP-PLAYER

This is actions and overlay that will play different types of clips.

- PlayTwitchClip
- PlayRandomTwitchClip
- PlayTikTokVideo


# REQUIREMENTS

This requires the HtmlAgilityPack library to be in the streamer.bot install directory.
Vote up on [this suggestion](https://ideas.streamer.bot/posts/159/nuget-support-for-c-execute-actions) to make this easier.

# ACTIONS

## PlayTwitchClip

This will play the clip that is sent in as `%input0%` or `%twitchVideo%`

A regex command that matches the following:

```regex
https:\/\/(?:www|clips)\.twitch\.tv\/(?:[^\/]+\/clip\/)?(?<id>.*?)(?:\?.*)?(?:\s|$)
```

## PlayRandomTwitchClip

This will play `N` number of random clips for a user that is specified via `%input0%` or `%targetUser%`

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

This will play a TikTok video that is specified via `%input0%` or `%tiktokVideo%`.

A regex command that matches the following:
```regex
https:\/\/(?:www.)?tiktok\.com\/@(.*?)\/video\/(.*?)(\?.*?|$|\s)
```