//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.EyeX.Client;

/// <summary>
/// Interface of EyeX behavior classes. Used by the EyeXHost.
/// </summary>
public interface IEyeXBehavior
{
    /// <summary>
    /// Assigns the behavior to an interactor object.
    /// </summary>
    /// <param name="interactor">The interactor object.</param>
    void AddTo(Interactor interactor);

    /// <summary>
    /// Handles an event addressed to the interactor.
    /// </summary>
    /// <param name="event_">The event object.</param>
    void HandleEvent(InteractionEvent event_);
}
