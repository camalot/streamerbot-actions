# SECRET WORD

An action that triggers when a secret word is said in the chat. A new word is selected at the start, and every time the previous word is found.

# COMMANDS

This needs one command. 

This needs to be a `Regex` command and the command needs to be `([^\s]+)`.

![](https://i.imgur.com/ElcFZCC.png)


# ACTIONS

- `SecretWord`: This action is the the one that checks if the word has been found, and will select the new word.

- `FoundSecretWord`: This action is called by `SecretWord` and is passed a `foundWord` argument. You can use this to notify chat that the word was found, and by whom.