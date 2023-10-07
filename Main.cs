using SuchByte.MacroDeck.Plugins;
using System.Collections.Generic;
using System;
using WindowsInput;
using System.Timers;
using System.Threading.Tasks;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Variables;
using Windows.Media.Control;

namespace MediaControls_Plugin
{
    public class MediaControlsPlugin : MacroDeckPlugin
    {
        private GlobalSystemMediaTransportControlsSessionManager _manager = null;
        private GlobalSystemMediaTransportControlsSession _session = null;
        Timer timeDateTimer;
        public override async void Enable()
        {
            this.Actions = new List<PluginAction>
            {
                new MediaPlayPauseAction(),
                new MediaNextAction(),
                new MediaPrevAction(),
                new MediaVolUpAction(),
                new MediaVolDownAction(),
                new MediaVolMuteAction(),
            };

            _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            if (_manager != null)
            {
                _manager.SessionsChanged += Manager_SessionsChanged;
            }
            Manager_SessionsChanged(null, null);

            this.timeDateTimer = new Timer(1000)
            {
                Enabled = true
            };
            this.timeDateTimer.Elapsed += this.OnTimerTick;
            this.timeDateTimer.Start();
        }
        private void Manager_SessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
        {
            if (_manager != null)
            {
                _session = _manager.GetCurrentSession();
                if (_session != null)
                {
                    GetCurrentPlayingTittleAsync(null, null);
                }
            }
            else
            {
                _session = null;
            }
        }
        private void OnTimerTick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                GetVolumeLevel();
                GetCurrentPlayingTittleAsync(null, null);
            });
        }
        async void GetCurrentPlayingTittleAsync(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
        {
            try
            {
                if (_session != null)
                {
                    try
                    {
                        var info = await _session.TryGetMediaPropertiesAsync();
                        if (info != null)
                        {
                            VariableManager.SetValue("CurrentPlayingTittle", info.Title, VariableType.String, this, null);
                        }
                    }
                    catch
                    {

                        VariableManager.SetValue("CurrentPlayingTittle", "err", VariableType.String, this, null);
                    }
                }
            }
            catch
            {
            }
        }

        private void GetVolumeLevel()
        {
            try
            {
                //Getting audio level and converting it to integer
                VariableManager.SetValue("VolumeLevel", Math.Round(AudioManager.GetMasterVolume()), VariableType.String, this, null);
            }catch
            {
                VariableManager.SetValue("VolumeLevel", "err", VariableType.String, this, null);
            }
        }
    }
    public class MediaPlayPauseAction : PluginAction
    {
        public override string Name => "Media Play/Pause";
        public override string Description => "Pauses or resume the current track on a media player.";
        public override void Trigger(string clientId, ActionButton actionButton)
        {
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
        }
    }

    public class MediaNextAction : PluginAction
    {
        public override string Name => "Media Next";
        public override string Description => "Plays the next track on a media player.";
        public override void Trigger(string clientId, ActionButton actionButton)
        {
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
        }
    }

    public class MediaPrevAction : PluginAction
    {
        public override string Name => "Media Prev";
        public override string Description => "Plays the previous track on a media player.\n\r\n\rConfiguration: no";
        public override void Trigger(string clientId, ActionButton actionButton)
        {
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);
        }
    }
    public class MediaVolUpAction : PluginAction
    {
        public override string Name => "Media Volume Up";
        public override string Description => "Increase volume on a media player.\n\r\n\rConfiguration: no";
        public override void Trigger(string clientId, ActionButton actionButton)
        {
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
        }
    }
    public class MediaVolDownAction : PluginAction
    {
        public override string Name => "Media Volume Down";
        public override string Description => "Decrease volume on a media player.\n\r\n\rConfiguration: no";
        public override void Trigger(string clientId, ActionButton actionButton)
        {
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VOLUME_DOWN);
        }
    }
    public class MediaVolMuteAction : PluginAction
    {
        public override string Name => "Media Volume Mute";
        public override string Description => "Mute volume on a media player.\n\r\n\rConfiguration: no";
        public override void Trigger(string clientId, ActionButton actionButton)
        {
            new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VOLUME_MUTE);
        }
    }
}

