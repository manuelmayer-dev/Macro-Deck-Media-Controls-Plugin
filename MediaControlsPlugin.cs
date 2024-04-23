using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System;
using System.Timers;
using System.Threading.Tasks;
using SuchByte.MacroDeck.Variables;
using Windows.Media.Control;

namespace MediaControls_Plugin;

public class MediaControlsPlugin : MacroDeckPlugin
{
    private GlobalSystemMediaTransportControlsSessionManager _manager;
    private GlobalSystemMediaTransportControlsSession _session;
    private Timer _timeDateTimer;

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
        };

        await InitializeSessionManager();

        _timeDateTimer = new Timer(1000)
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
            VariableManager.SetValue("volume_level", volume, VariableType.Integer, this, null);
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
            VariableManager.SetValue("volume_muted", isMuted, VariableType.Bool, this, null);
        }
    }
}