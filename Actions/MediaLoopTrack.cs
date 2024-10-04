using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Control;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;


// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaLoopTrack : PluginAction
{
    private GlobalSystemMediaTransportControlsSessionManager _manager;

    public MediaLoopTrack()
    {
        _manager = MediaControlsPlugin.Manager;
    }
    public override string Name => "Media Loop Track";
    public override string Description => "Turns on looping for the current track on a media player.\n\r\n\rConfiguration: no";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var session = _manager.GetCurrentSession();
        if (session == null)
        {
            return;
        }
        Task.Run(async () => await session.TryChangeAutoRepeatModeAsync(MediaPlaybackAutoRepeatMode.Track));
    }
}