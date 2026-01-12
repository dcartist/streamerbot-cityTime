# streamerbot-cityTime

This is a C# script to show the city's time
using the geonames api

Register for geonames  api

Example:

some_twitch_user: !time los angeles
Response_bot: 🕒 The current time in Los Angeles is: Monday, January 12, 2026 - 11:05 AM (America/Los_Angeles)


In streamer bot create a command:
- Name: !time
- Command !time
- Sources: Twitch Message

  
For Actions & Queues:

- Name: Get Time
- Under Triggers go to Add > Core > Commands > Command Triggered
- Under subaction use Twitch > GetUser info for target insert ```%rawInput%``` in the field for User Login
- Under subaction use Execute Core (timer) and add the code from the repo into the area and change the user name to what you have registered for
