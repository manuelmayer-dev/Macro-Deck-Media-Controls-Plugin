using System;
using System.Threading.Tasks;
using Windows.Media.Control;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Variables;


// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaShuffleToggle : PluginAction
{
    private GlobalSystemMediaTransportControlsSessionManager _manager;

    public MediaShuffleToggle()
    {
        _manager = MediaControlsPlugin.Manager;
    }
    public override string Name => "Media Shuffle Toggle";
    public override string Description => "Toggles the shuffle for the current media player.\n\r\n\rConfiguration: no";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var session = _manager.GetCurrentSession();
        if (session == null)
        {
            return;
        }
        var shuffle = Task.Run(session.GetPlaybackInfo).GetAwaiter().GetResult()?.IsShuffleActive ?? false; 
        Task.Run(async () => await session.TryChangeShuffleActiveAsync(!shuffle));
    }
}