using System;
using System.Threading.Tasks;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using Windows.Media.Control;

// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaPlayPauseAction : PluginAction
{
    private GlobalSystemMediaTransportControlsSessionManager _manager;
    public MediaPlayPauseAction()
    {
        if (PluginManager.Plugins.ContainsKey("Macro Deck Media Controls"))
        {
            MacroDeckPlugin plugin = PluginManager.Plugins["Macro Deck Media Controls"];
            if (plugin.GetType().IsEquivalentTo(typeof(MediaControlsPlugin)))
            {
                _manager = ((MediaControlsPlugin)plugin).Manager;
            }
        }
    }
    public override string Name => "Media Play/Pause";
    public override string Description => "Pauses or resume the current track on a media player.";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var session = _manager.GetCurrentSession();
        if (session == null)
        {
            return;
        }
        var test = Task.Run(async () => await session.TryTogglePlayPauseAsync()).GetAwaiter().GetResult();
    }
}