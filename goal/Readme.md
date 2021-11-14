# GOAL OVERLAY ACTIONS

[![](https://i.imgur.com/jEj3vrPm.png)](https://i.imgur.com/9J0dYEw.mp4)

This is [Streamer.Bot](https://streamer.bot) actions with overlay to track revenue for a goal.

These actions can be imported in to streamer.bot from the file: `./goal.action`

![](https://i.imgur.com/DaAo0Ib.png)

# ADDITIONAL STREAMER.BOT SETUP

## ACTION

Set the global variables for `gpb_current_file` and `gpb_goal_file` in the `GoalInit` action. These are the path to the files that store what the current value is for the running goal.

Change the `GoalIncrement` sub-actions to show/hide/play

![](https://i.imgur.com/PsRIYEn.png)

## GoalIncrement OUTPUT ARGUMENTS

- `gpbIncrease`: The numeric value of how much it is increasing .
- `gpbIncreaseFormatted`: The formatted value of how much it is increasing.
- `gpbTotal`: The numeric value for the current total.
- `gpbTotalFormatted`: The formatted value for the current total.
- `gpbGoal`: The numeric value for the goal.
- `gpbGoalFormatted`: The formatted value for the goal.


## COMMANDS

Create a command called `!gma` that only allows the broadcaster to apply (or moderators if you want to allow them to increase the total). This command is for situations for other types of revenue that might not have events tied to them. For example, someone tips you via CashApp.

![](https://i.imgur.com/hUfY53N.png)

## SERVERS/CLIENTS

This needs to use both the `Websocket Server` and the custom `Websocket Servers` options. It uses the built-in to get the events sent to it when triggered through the actions. The other is used just to get the initial values when first connected. Since the streamer.bot does not have a `connected` event action like the custom servers do.

The addresses for these 2 websocket servers need to be set in `./settings.js`.

![](https://i.imgur.com/b3XTsOY.png)

![](https://i.imgur.com/VJUeJYz.png)

## EVENTS

Now we need to set up the events to trigger the goal actions.

First setup the Streamlabs Tip Event  
![](https://i.imgur.com/B4RB9iw.png)

Next is StreamElements Tip Event  
![](https://i.imgur.com/gNffaNt.png)

Now set up for Cheers/Bits  
![](https://i.imgur.com/kMZYa3z.png)

Set up Sub/Re-Sub/Gift Sub/Gift Bomb with the following. 

Making sure to set the action for each Tier Type:

- Prime: GoalIncSubTier1
- Tier1: GoalIncSubTier1
- Tier2: GoalIncSubTier2
- Tier2: GoalIncSubTier3

![](https://i.imgur.com/OTlZREw.png)

Finally, we need to set up a file watcher for `%gpb_current_file%` and `%gpb_goal_file%` files and set their Action to be `GoalInit`.

![](https://i.imgur.com/k67Qb93.png)

# OBS / SLOBS CONFIGURATION

## PROGRESS BAR OVERLAY
Add a browser source and point it to your path of `goal/goal.html`. This should **NOT** be a `local file`. You should use a `file://` path like the following.

![](https://i.imgur.com/D5Dge8f.png)  

Make sure the height is `128` and the width is at least `1200`. Scale the source down in OBS/SLOBS after you add it.

## VALUE OVERLAY
Add a browser source and point it to your path of `goal/value.html`. This should **NOT** be a `local file`. You should use a `file://` path like the following.

![](https://i.imgur.com/Zshitg1.png)  


Make sure the height is `128` and the width is `300`. Scale the source down in OBS/SLOBS after you add it.


# SETTINGS

Edit the values in `settings.js`

```jsonc
{
  "FallBackFonts": [ "Roboto", "Droid Sans", "Droid Serif" ], // additional fonts to load
  "FontName": "Permanent Marker", // primary google web font name to use as the font
  "Label": "!goal", // the label used in both progress bar and values overlay

  "SB_WebSocket": "ws://127.0.0.1:1377", // the sb websocket address
  "SB_CustomWebSocket": "ws://127.0.0.1:4141/goal" // the custom websocket address
}

```