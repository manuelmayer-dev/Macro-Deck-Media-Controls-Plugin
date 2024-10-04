# Media Controls plugin for Macro Deck 2
This plugin can control most of the media players (next, previous, play/pause)

# Features
- ### Next track
- ### Previous track
- ### Play/pause
- ### Volume control
- ### Fast Forward/Rewind
- ### Shuffle
- ### Repeat
  - ### Single Track
  - ### Whole Playlist

# Variables
| Name                     | Description                                                                                                           | Type   | Default   |
|--------------------------|-----------------------------------------------------------------------------------------------------------------------|--------|-----------|
| `current_playing_title`  | The current playing track title                                                                                       | `str`  | `Unknown` |
| `current_playing_artist` | The current playing track artist                                                                                      | `str`  | `Unknown` |
| `volume_level`           | The current volume level                                                                                              | `int`  | `0`       |
| `volume_muted`           | Whether the player is muted                                                                                           | `bool` | `false`   |
| `is_playing`             | Whether the player is playing                                                                                         | `bool` | `false`   |
| `is_shuffled`            | Whether the player is shuffled                                                                                        | `bool` | `false`   |
| `playback_status`        | The current playback status <br/> 0. Closed<br/>1. Opened<br/>2. Changing<br/>3. Stopped<br/>4. Playing<br/>5. Paused | `int`  | `0`       |
| `loop_status`            | The current loop status <br/> 0. Off<br/>1. Track<br/>2. Playlist                                                     | `int`  | `0`       |

### This is a plugin for [Macro Deck 2](https://github.com/SuchByte/Macro-Deck), it does NOT function as a standalone app
<img height="64px" src="https://macrodeck.org/images/macro_deck_2_official_plugin.png"  alt="Official Macro Deck 2 Plugin"/>


# Third party licenses
This plugin uses some awesome 3rd party libraries:
- [H.InputSimulator (MS-PL)](https://github.com/HavenDV/H.InputSimulator)
