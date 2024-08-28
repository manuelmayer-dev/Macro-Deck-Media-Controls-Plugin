using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System;
using System.Timers;
using System.Threading.Tasks;
using Windows.Media;
using SuchByte.MacroDeck.Variables;
using Windows.Media.Control;


namespace MediaControls_Plugin;

public class MediaControlsPlugin : MacroDeckPlugin
{
    public GlobalSystemMediaTransportControlsSessionManager Manager;
    private GlobalSystemMediaTransportControlsSession _session;
    private Timer _timeDateTimer;

    public override async void Enable()
    {
        await InitializeSessionManager();

        _timeDateTimer = new Timer(1000)
        {
            Enabled = true
        };
        _timeDateTimer.Elapsed += OnTimerTick;
        _timeDateTimer.Start();
        Actions = new List<PluginAction>
        {
            new MediaPlayPauseAction(),
            new MediaNextAction(),
            new MediaPrevAction(),
            new MediaVolUpAction(),
            new MediaVolDownAction(),
            new MediaVolMuteAction(),
            new MediaFastForwardAction(),
            new MediaRewindAction(),
            new MediaShuffleOn(),
            new MediaShuffleOff(),
            new MediaLoopOff(),
            new MediaLoopList(),
            new MediaLoopTrack(),
            
        };
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
            VariableManager.SetValue("is_shuffle", shuffle, VariableType.Bool, this, null!);
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
        var volume = 0d;
        try
        {
            volume = Math.Round(AudioManager.GetMasterVolume());
        }
        catch
        {
            volume = 0;
        }
        finally
        {
            VariableManager.SetValue("volume_level", volume, VariableType.Integer, this, null!);
        }
    }

    private void UpdateVolumeMuteState()
    {
        var isMuted = false;
        try
        {
            isMuted = AudioManager.GetMasterVolumeMute();
        }
        catch
        {
            isMuted = false;
        }
        finally
        {
            VariableManager.SetValue("volume_muted", isMuted, VariableType.Bool, this, null!);
        }
    }
}