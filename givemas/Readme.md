# Givemas Goal

This is [Streamer.Bot](https://streamer.bot) actions with overlay to track revenue for what I call givemas.

Givemas is from November 1st -> December 25th. All generated revenue to my stream, via bits, subscriptions, and tips, are added together. On December 25th, I give a person working in the service industry, like a waitress, or delivery person, the amount that was generated. 

These actions can be imported in to streamer.bot from the file: `./givemas.action`

![](https://i.imgur.com/5oOcV8U.png)

# ADDITIONAL STREAMER.BOT SETUP

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

Prime: GivemasIncSubTier1
Tier1: GivemasIncSubTier1
Tier2: GivemasIncSubTier2
Tier2: GivemasIncSubTier3

![](https://i.imgur.com/Ok3fpez.png)

Finally, we need to set up a file watcher for the `givemas_current.txt` and `givemas_goal.txt` and set their Action to be `GivemasInit`.

![](https://i.imgur.com/k67Qb93.png)

The location of these files need to be changed in the `IncrementGivemas` and `GivemasInit` C# Execute Code Action.  
![](https://i.imgur.com/4ZeyL9z.png)