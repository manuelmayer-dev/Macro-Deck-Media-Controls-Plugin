using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using WindowsInput;

// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaNextAction : PluginAction
{
    public override string Name => "Media Next";
    public override string Description => "Plays the next track on a media player.";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.MEDIA_NEXT_TRACK);
    }
}