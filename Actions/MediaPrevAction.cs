using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using WindowsInput;

// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaPrevAction : PluginAction
{
    public override string Name => "Media Prev";
    public override string Description => "Plays the previous track on a media player.\n\r\n\rConfiguration: no";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.MEDIA_PREV_TRACK);
    }
}