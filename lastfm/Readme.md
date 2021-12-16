# LASTFM

This will get the info of a currently playing track from lastfm.

## INSTALL

Import `lastfm/lastfm.action` in streamer.bot.

## INPUT VARIABLES
| NAME | DESCRIPTION | DEFAULT | 
| --- | --- | --- |
| `lastFmUser` | The user to get track info for | `""` | 

## OUTPUT VARIABLES

These are arguments that are available after calling the action.

| NAME | DESCRIPTION | DEFAULT | 
| --- | --- | --- |
| `lastfm_playing` | boolean to show if lastfm data is currently available | `false` | 
| `lastfm_title` | The title of the track playing | `""` | 
| `lastfm_artist` | The artist of the track playing | `""` | 
| `lastfm_album` | The album name of the track that is playing | `""` | 
| `lastfm_image` | The album art for the track playing | `""` | 

`NOTE: album is not populated if you use YouTube Music Desktop`

## SAMPLE CONFIGURATION
![](https://i.imgur.com/hwkyIOB.png)

## SAMPLE OUTPUT
![](https://i.imgur.com/RzHLkyy.png)
