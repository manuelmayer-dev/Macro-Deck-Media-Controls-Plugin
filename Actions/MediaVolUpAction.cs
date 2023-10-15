using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using WindowsInput;

// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaVolUpAction : PluginAction
{
    public override string Name => "Media Volume Up";
    public override string Description => "Increase volume on a media player.\n\r\n\rConfiguration: no";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.VOLUME_UP);
    }
}