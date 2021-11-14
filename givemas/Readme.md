# Givemas Goal

[![](https://i.imgur.com/jEj3vrPm.png)](https://i.imgur.com/9J0dYEw.mp4)

This is [Streamer.Bot](https://streamer.bot) actions with overlay to track revenue for what I call givemas.

Givemas is from November 1st -> December 25th. All generated revenue to my stream, via bits, subscriptions, and tips, are added together. On December 25th, I give a person working in the service industry, like a waitress, or delivery person, the amount that was generated. 

These actions can be imported in to streamer.bot from the file: `./givemas.action`

![](https://i.imgur.com/5oOcV8U.png)

# ADDITIONAL STREAMER.BOT SETUP

## ACTION


Change the `IncrementGivemas` sub-actions to show/hide/play

![](https://i.imgur.com/PsRIYEn.png)



## IncrementGivemas OUTPUT ARGUMENTS

- `gmgIncrease`: The numeric value of how much it is increasing .
- `gmgIncreaseFormatted`: The formatted value of how much it is increasing.
- `gmgTotal`: The numeric value for the current total.
- `gmgTotalFormatted`: The formatted value for the current total.
- `gmgGoal`: The numeric value for the goal.
- `gmgGoalFormatted`: The formatted value for the goal.


## COMMANDS

Create a command called `!gma` that only allows the broadcaster to apply (or moderators if you want to allow them to increase the total). This command is for situations for other types of revenue that might not have events tied to them. For example, someone tips you via CashApp.

![](https://i.imgur.com/OinXgQ0.png)

## SERVERS/CLIENTS

This needs to use both the `Websocket Server` and the custom `Websocket Servers` options. It uses the built-in to get the events sent to it when triggered through the actions. The other is used just to get the initial values when first connected. Since the streamer.bot does not have a `connected` event action like the custom servers do.

The addresses for these 2 websocket servers need to be set in `./settings.js`.

![](https://i.imgur.com/b3XTsOY.png)

![](https://i.imgur.com/HEF96Wt.png)

## EVENTS

Now we need to set up the events to trigger the givemas actions.

First setup the Streamlabs Tip Event  
![](https://i.imgur.com/i9LJOcn.png)

Next is StreamElements Tip Event  
![](https://i.imgur.com/jNfMSKP.png)

Now set up for Cheers/Bits  
![](https://i.imgur.com/7AOnQi6.png)

Set up Sub/Re-Sub/Gift Sub/Gift Bomb with the following. 

Making sure to set the action for each Tier Type:

- Prime: GivemasIncSubTier1
- Tier1: GivemasIncSubTier1
- Tier2: GivemasIncSubTier2
- Tier2: GivemasIncSubTier3

![](https://i.imgur.com/Ok3fpez.png)

Finally, we need to set up a file watcher for the `givemas_current.txt` and `givemas_goal.txt` and set their Action to be `GivemasInit`.

![](https://i.imgur.com/k67Qb93.png)

The location of these files need to be changed in the `IncrementGivemas` and `GivemasInit` C# Execute Code Action.  
![](https://i.imgur.com/4ZeyL9z.png)

# OBS / SLOBS CONFIGURATION

## PROGRESS BAR OVERLAY
Add a browser source and point it to your path of `givemas/goal.html`. This should **NOT** be a `local file`. You should use a `file://` path like the following.

![](https://i.imgur.com/D5Dge8f.png)  

Make sure the height is `128` and the width is `1920`. Scale the source down in OBS/SLOBS after you add it.

## VALUE OVERLAY
Add a browser source and point it to your path of `givemas/value.html`. This should **NOT** be a `local file`. You should use a `file://` path like the following.

![](https://i.imgur.com/Zshitg1.png)  


Make sure the height is `128` and the width is `300`. Scale the source down in OBS/SLOBS after you add it.


# SETTINGS

Edit the values in `settings.js`

```jsonc
{
  "FallBackFonts": [ "Roboto", "Droid Sans", "Droid Serif" ], // additional fonts to load
  "FontName": "Permanent Marker", // primary google web font name to use as the font
  "Label": "!givemas", // the label used in both progress bar and values overlay

  "SB_WebSocket": "ws://127.0.0.1:1377", // the sb websocket address
  "SB_CustomWebSocket": "ws://127.0.0.1:4141/givemas" // the custom websocket address
}

```