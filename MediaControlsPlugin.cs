using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System;
using System.Timers;
using System.Threading.Tasks;
using SuchByte.MacroDeck.Variables;
using Windows.Media.Control;

namespace MediaControls_Plugin;

public static class PluginInstance
{
    public static MediaControlsPlugin Main;
}

public class MediaControlsPlugin : MacroDeckPlugin
{
    private GlobalSystemMediaTransportControlsSessionManager _manager;
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
    }

    private async Task InitializeSessionManager()
    {
        _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        if (_manager != null)
        {
            _manager.SessionsChanged += Manager_SessionsChanged;
        }

        await UpdateSession();
    }

    private async Task UpdateSession()
    {
        if (_manager != null)
        {
            _session = _manager.GetCurrentSession();
            if (_session != null)
            {
                await UpdatePlayingTitleAsync();
            }
        }
        else
        {
            _session = null;
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
            VariableManager.SetValue("current_playing_title", currentTile, VariableType.String, this, null);
            VariableManager.SetValue("current_playing_artist", currentArtist, VariableType.String, this, null);
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