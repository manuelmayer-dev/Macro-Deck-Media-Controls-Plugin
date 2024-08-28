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
        if (PluginManager.Plugins.ContainsKey("Macro Deck Media Controls"))
        {
            MacroDeckPlugin plugin = PluginManager.Plugins["Macro Deck Media Controls"];
            if (plugin.GetType().IsEquivalentTo(typeof(MediaControlsPlugin)))
            {
                _manager = ((MediaControlsPlugin)plugin).Manager;
            }
        }
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
        var test = Task.Run(async () => await session.TrySkipNextAsync()).GetAwaiter().GetResult();
    }
}