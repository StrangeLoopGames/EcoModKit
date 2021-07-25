Eco Discord Access Mod
======================

This mod provides Discord role based access to Eco servers. Allowing for more automatic control of who can access your community Eco servers. This mod can also be bundled with the native Discord Twitch integration and the Twitch Subscriber role to create subscriber only Eco servers.

This mod was originally created to demostrate the usage of the new IUserAuthorizer interface in Eco 9.5+. Any long term support or reliability cannot be guaranteed.

## Configuring your Discord bot

This mod takes a standard Bot token to interact with Discord. However it does require the following gateway intents be enabled.

* Presence Intent
* Server Members Intent

## Linking your Discord user to Eco

To create a link between your Discord user and Eco new users must run the following command before they can access the Eco server.
```
!ecolink [eco username/steam username]
```

This will create a database link between their Discord user/roles and the Eco game server