# DiscordBot.netcore
Discord bot build with DSharpplus library, Written in C#.

## Commands
### Slash Commands
#### Music
| Command  | Description                                              |
| -------- | -------------------------------------------------------- |
| `/join`  | Make bot join the voice channel that you are currently in|
| `/play`  | Play song from the given URL (Youtube or Soundcloud)     |
| `/pause` | Pause the song that are currently playing                |
| `/dis`   | Disconnect the bot from the channel that it currently in |
| `/loop`  | Loop the song that are currently playing                 |

### Non-Slash Commands
| Command  | Description                                       |
| -------- | ------------------------------------------------- |
| `vote`   | Vote to disconnect a member in the voice channel  |
| `member` | Get amount of member currently in a voice channel |

## Setup

### Lavalink
RUN Lavalink before starting the bot or the music commands will not work.
1. Download [Lavalink](https://github.com/freyacodes/Lavalink/releases)
2. Open the terminal in the Lavalink directory and run this command: 
```
java -jar Lavalink.jar
```

### Config
add config.json file to the root directory of the project.
```
{
  "token": "{your token here}",
  "prefix": "?"//prefix for the commands
}
```
