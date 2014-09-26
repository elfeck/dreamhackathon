﻿//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

/// <summary>
/// Base class for data streams.
/// </summary>
/// <typeparam name="T">Type of the provided data value object.</typeparam>
public abstract class EyeXDataStreamBase<T> : IEyeXDataProvider<T>, IEyeXGlobalInteractor
{
    private int _usageCount;

    /// <summary>
    /// Event raised when the state of the global interactor has changed
    /// in a way so that the EyeX Engine has to be updated with the new
    /// parameter settings.
    /// </summary>
    public event EventHandler Updated;

    private bool IsStarted
    {
        get { return _usageCount > 0; }
    }

    /// <summary>
    /// Gets or sets the latest value of the data stream. The value is never null but 
    /// it might be invalid.
    /// </summary>
    public T Last { get; protected set; }

    /// <summary>
    /// Gets the unique ID of the interactor that provides the data stream.
    /// </summary>
    public abstract string Id
    {
        get;
    }

    /// <summary>
    /// Starts the provider. Data will continuously be updated in the Last
    /// property as events are received from the EyeX Engine.
    /// </summary>
    public void Start()
    {
        _usageCount++;
        if (_usageCount == 1)
        {
            OnStreamingStarted();
            RaiseUpdatedEvent();
        }
    }

    /// <summary>
    /// Requests to stop the data provider. If there are no other clients
    /// that are currently requesting the provider to keep providing data,
    /// the provider will stop the stream of data from the EyeX Engine and
    /// stop updating the Last property.
    /// </summary>
    public void Stop()
    {
        _usageCount--;
        if (_usageCount == 0)
        {
            OnStreamingStopped();
            RaiseUpdatedEvent();
        }
    }

    /// <summary>
    /// Creates an EyeX Interactor object from the properties and behaviors of
    /// this global interactor and adds it to the provided snapshot.
    /// </summary>
    /// <param name="snapshot">The <see cref="Snapshot"/> to
    /// add the interactor to.</param>
    /// <param name="forceDeletion">If true, forces the interactor to be deleted.</param>
    public void AddToSnapshot(Snapshot snapshot, bool forceDeletion)
    {
        var interactor = snapshot.CreateInteractor(Id, Literals.RootId, Literals.GlobalInteractorWindowId);
        interactor.CreateBounds(BoundsType.None);

        if (!IsStarted || forceDeletion)
        {
            interactor.IsDeleted = true;
        }

        AssignBehavior(interactor);
    }

    /// <summary>
    /// Handles interaction events.
    /// </summary>
    /// <param name="event_">The <see cref="InteractionEvent"/> instance containing the event data.</param>
    /// <param name="gameWindowPosition">The position of the top-left corner of the game window, in operating system coordinates.</param>
    /// <param name="horizontalScreenScale">The horizontal relationship between the Unity and operating system coordinate systems.</param>
    /// <param name="verticalScreenScale">The vertical relationship between the Unity and operating system coordinate systems.</param>
    public abstract void HandleEvent(InteractionEvent event_, Vector2 gameWindowPosition, float horizontalScreenScale, float verticalScreenScale);

    protected abstract void AssignBehavior(Interactor interactor);

    protected virtual void OnStreamingStarted()
    {
        // default implementation does nothing
    }

    protected virtual void OnStreamingStopped()
    {
        // default implementation does nothing
    }

    private void RaiseUpdatedEvent()
    {
        var handler = Updated;
        if (null != handler)
        {
            handler(this, EventArgs.Empty);
        }
    }
}
