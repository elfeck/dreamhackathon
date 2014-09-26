//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

/// <summary>
/// Provider of eye position data. When the provider has been started it
/// will continuously update the Last property with the latest gaze point 
/// value received from the EyeX Engine.
/// </summary>
public class EyeXEyePositionDataStream : EyeXDataStreamBase<EyeXEyePosition>
{
    /// <summary>
    /// Creates a new instance.
    /// Note: don't create instances of this class directly. Use the <see cref="EyeXHost.GetEyePositionDataProvider"/> method instead.
    /// </summary>
    public EyeXEyePositionDataStream()
    {
        Last = EyeXEyePosition.Invalid;
    }

    public override string Id
    {
        get { return "EyeXEyePositionDataStream"; }
    }

    protected override void AssignBehavior(Interactor interactor)
    {
        interactor.CreateBehavior(BehaviorType.EyePositionData);
    }

    public override void HandleEvent(InteractionEvent event_, Vector2 gameWindowPosition, float horizontalScreenScale, float verticalScreenScale)
    {
        // Note that this method is called on a worker thread, so we MAY NOT access any game objects from here.
        // The data is stored in the Last property and used from the main thread.
        foreach (var behavior in event_.Behaviors)
        {
            if (behavior.BehaviorType != BehaviorType.EyePositionData) { continue; }

            EyePositionDataEventParams eventParams;
            if (behavior.TryGetEyePositionDataEventParams(out eventParams))
            {
                var left = new EyeXSingleEyePosition(eventParams.HasLeftEyePosition != EyeXBoolean.False, (float)eventParams.LeftEyeX, (float)eventParams.LeftEyeY, (float)eventParams.LeftEyeZ);
                var right = new EyeXSingleEyePosition(eventParams.HasRightEyePosition != EyeXBoolean.False, (float)eventParams.RightEyeX, (float)eventParams.RightEyeY, (float)eventParams.RightEyeZ);
                Last = new EyeXEyePosition(left, right, eventParams.Timestamp);
            }
        }
    }

    protected override void OnStreamingStarted()
    {
        Last = EyeXEyePosition.Invalid;
    }
}
