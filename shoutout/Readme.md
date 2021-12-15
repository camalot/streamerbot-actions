# SHOUTOUT OVERLAY

This is [Streamer.Bot](https://streamer.bot) actions with overlay to track revenue for a goal.

These actions can be imported in to streamer.bot from the file: `./shoutout.action` in the release package

## ARGUMENTS

These arguments can be added to the action to change the display of the overlay. If they are not set, the defaults will be used

| NAME | DESCRIPTION | DEFAULT |
| --- | --- | --- |
| `duration` | The length in seconds to show the info | `10` |
| `fontName` | The font name to use. Either a font on your machine, or a [Google Font](https://fonts.google.com/) | `Bakbak One` |
| `inTransition` | The transition animation when coming in. [Read More](https://animate.style/) | `slideInRight` |
| `outTransition` | The transition animation when leaving. [Read More](https://animate.style/) | `slideOutLeft` |
| `attentionAnimation` | The animation to get attention. [Read More](https://animate.style/) | `none` |
| `enableShadow` | Enable a shadow around the text. | `true` |
| `userColor` | The color of the user's name | `rgb(255,255,255,1)` |
| `linkColor` | The color of the link text | `rgb(255,0,0,1)` |
| `imageShape` | Profile image shape: `rounded,square,circle` | `circle` |
| `showLink` | Disable / Enable the visibility of the link text line | `true` |
| `linkText` | The text line below the users name. `{username}` will be replaced with the user's name. | `twitch.tv/{username}`
## Command

Create a command, for example `!so`, and set the action to be the `Shoutout` action.


## OBS / SLOBS CONFIGURATION

### ADD THE OVERLAY
Add a browser source and point it to your path of `shoutout/overlay.html`. This should **NOT** be a `local file`. You should use a `file://` path like the following.

![](https://i.imgur.com/D5Dge8f.png)  

Set the width/height to your canvas size. example: 1920x1080
