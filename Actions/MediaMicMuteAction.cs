using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;
using SuchByte.MacroDeck.Logging;
using SuchByte.MacroDeck.Variables;

// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaMicMuteAction : PluginAction
{
    private AudioManager microphoneManager = new AudioManager(Mode.Microphone);
    public override string Name => "Media Microphone Mute";
    public override string Description => "Mute microphone";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var state = microphoneManager.GetMasterVolumeMute();
        var newState = !state;
        MacroDeckLogger.Trace(PluginInstance.Main, "Is microphone muted: " + state + ", setting to: " + newState);
        microphoneManager.SetMasterVolumeMute(newState);
        VariableManager.SetValue("mic_muted", newState, VariableType.Bool, PluginInstance.Main, null);
    }
}