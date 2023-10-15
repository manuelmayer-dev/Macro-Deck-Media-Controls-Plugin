using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using WindowsInput;

// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaPlayPauseAction : PluginAction
{
    public override string Name => "Media Play/Pause";
    public override string Description => "Pauses or resume the current track on a media player.";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.MEDIA_PLAY_PAUSE);
    }
}