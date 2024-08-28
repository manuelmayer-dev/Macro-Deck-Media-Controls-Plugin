using System;
using System.Threading.Tasks;
using Windows.Media.Control;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;


// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaNextAction : PluginAction
{
    private readonly GlobalSystemMediaTransportControlsSessionManager _manager;

    public MediaNextAction()
    {
        _manager = MediaControlsPlugin.Manager;
    }
    public override string Name => "Media Next";
    public override string Description => "Plays the next track on a media player.";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var session = _manager.GetCurrentSession();
        if (session == null)
        {
            return;
        }

        Task.Run(async () => await session.TrySkipNextAsync());
    }
}