using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System;
using System.Timers;
using System.Threading.Tasks;
using Windows.Media;
using SuchByte.MacroDeck.Variables;
using Windows.Media.Control;


namespace MediaControls_Plugin;

public static class PluginInstance
{
    public static MediaControlsPlugin Main;
}

public class MediaControlsPlugin : MacroDeckPlugin
{
    public static GlobalSystemMediaTransportControlsSessionManager Manager;
    private GlobalSystemMediaTransportControlsSession _session;
    private Timer _timeDateTimer;

    private AudioManager SpeakerManager = new AudioManager(Mode.Speakers);
    private AudioManager MicrophoneManager = new AudioManager(Mode.Microphone);

    public MediaControlsPlugin()
    {
        PluginInstance.Main = this;
    }

    public override async void Enable()
    {
        Actions = new List<PluginAction>
        {
            new MediaPlayPauseAction(),
            new MediaNextAction(),
            new MediaPrevAction(),
            new MediaVolUpAction(),
            new MediaVolDownAction(),
            new MediaVolMuteAction(),
            new MediaMicMuteAction(),
            new MediaFastForwardAction(),
            new MediaRewindAction(),
            new MediaShuffleToggle(),
            new MediaLoopOff(),
            new MediaLoopList(),
            new MediaLoopTrack(),
        };

        await InitializeSessionManager();

        _timeDateTimer = new Timer(300)
        {
            Enabled = true
        };
        _timeDateTimer.Elapsed += OnTimerTick;
        _timeDateTimer.Start();
    }

    private async void Manager_SessionsChanged(
        GlobalSystemMediaTransportControlsSessionManager sender,
        SessionsChangedEventArgs args)
    {
        await UpdateSession();
    }

    private async void OnTimerTick(object sender, EventArgs e)
    {
        UpdateVolumeLevel();
        UpdateVolumeMuteState();
        await UpdatePlayingTitleAsync();
        await UpdatePlayerStateAsync();
    }

    private async Task InitializeSessionManager()
    {
        Manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        if (Manager != null)
        {
            Manager.SessionsChanged += Manager_SessionsChanged;
        }

        await UpdateSession();
    }

    private async Task UpdateSession()
    {
        if (Manager != null)
        {
            _session = Manager.GetCurrentSession();
            if (_session != null)
            {
                await UpdatePlayingTitleAsync();
                await UpdatePlayerStateAsync();
            }
        }
        else
        {
            _session = null;
        }
    }

    private async Task UpdatePlayerStateAsync()
    {
        if (_session == null)
        {
            return;
        }

        var isPlaying = false;
        var playbackStatus = GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed;
        var shuffle = false;
        var loop = MediaPlaybackAutoRepeatMode.None;
        
        try
        {
            var info = await Task.Run(_session.GetPlaybackInfo);
            playbackStatus = info?.PlaybackStatus ?? GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed;
            isPlaying = playbackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;
            shuffle = info?.IsShuffleActive ?? false;
            loop = info?.AutoRepeatMode ?? MediaPlaybackAutoRepeatMode.None;
        }
        catch
        {
            isPlaying = shuffle = false;
            playbackStatus = GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed;
            loop = MediaPlaybackAutoRepeatMode.None;
        }
        finally
        {
            VariableManager.SetValue("is_playing", isPlaying, VariableType.Bool, this, null!);
            VariableManager.SetValue("is_shuffled", shuffle, VariableType.Bool, this, null!);
            VariableManager.SetValue("playback_status", (int) playbackStatus, VariableType.Integer, this, null!);
            VariableManager.SetValue("loop_status", (int) loop, VariableType.Integer, this, null!);
        }

    }
    private async Task UpdatePlayingTitleAsync()
    {
        if (_session == null)
        {
            return;
        }

        var currentTile = string.Empty;
        var currentArtist = string.Empty;
        
        try
        {
            var info = await _session.TryGetMediaPropertiesAsync();
            currentTile = info?.Title ?? "-";
            currentArtist = info?.Artist ?? "-";
        }
        catch
        {
            currentTile = currentArtist = "Unknown";
        }
        finally
        {
            VariableManager.SetValue("current_playing_title", currentTile, VariableType.String, this, null!);
            VariableManager.SetValue("current_playing_artist", currentArtist, VariableType.String, this, null!);
        }
    }

    private void UpdateVolumeLevel()
    {
        var speakerVolume = 0d;
        var micVolume = 0d;
        try
        {
            speakerVolume = Math.Round(SpeakerManager.GetMasterVolume());
            micVolume = Math.Round(MicrophoneManager.GetMasterVolume());
        }
        catch
        {
            speakerVolume = 0;
            micVolume = 0;
        }
        finally
        {
            VariableManager.SetValue("speaker_volume_level", speakerVolume, VariableType.Integer, this, null);
            VariableManager.SetValue("mic_volume_level", micVolume, VariableType.Integer, this, null);
        }
    }

    private void UpdateVolumeMuteState()
    {
        var isSpeakerMuted = false;
        var isMicMuted = false;
        try
        {
            isSpeakerMuted = SpeakerManager.GetMasterVolumeMute();
            isMicMuted = MicrophoneManager.GetMasterVolumeMute();
        }
        catch
        {
            isSpeakerMuted = false;
            isMicMuted = false;
        }
        finally
        {
            VariableManager.SetValue("speaker_muted", isSpeakerMuted, VariableType.Bool, this, null);
            VariableManager.SetValue("mic_muted", isMicMuted, VariableType.Bool, this, null);
        }
    }
}