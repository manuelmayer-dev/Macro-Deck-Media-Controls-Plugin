﻿using System;
using System.Threading.Tasks;
using Windows.Media.Control;
using SuchByte.MacroDeck.ActionButton;
using SuchByte.MacroDeck.Plugins;


// ReSharper disable once CheckNamespace
namespace MediaControls_Plugin; // Don't change because of compatibility

public class MediaFastForwardAction : PluginAction
{
    private GlobalSystemMediaTransportControlsSessionManager _manager;

    public MediaFastForwardAction()
    {
        _manager = MediaControlsPlugin.Manager;
    }
    public override string Name => "Media Fast Forward";
    public override string Description => "Fast forwards the current track on a media player.\n\r\n\rConfiguration: no";
    public override void Trigger(string clientId, ActionButton actionButton)
    {
        var session = _manager.GetCurrentSession();
        if (session == null)
        {
            return;
        } 
        Task.Run(async () => await session.TryFastForwardAsync());
    }
}