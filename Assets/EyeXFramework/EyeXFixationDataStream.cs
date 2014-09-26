//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

/// <summary>
/// Provider of fixation data. When the provider has been started it
/// will continuously update the Last property with the latest gaze point 
/// value received from the EyeX Engine.
/// </summary>
public class EyeXFixationDataStream : EyeXDataStreamBase<EyeXFixationPoint>
{
    private readonly FixationDataMode _mode;

    /// <summary>
    /// Creates a new instance.
    /// Note: don't create instances of this class directly. Use the <see cref="EyeXHost.GetFixationDataProvider"/> method instead.
    /// </summary>
    /// <param name="mode">Data mode.</param>
    public EyeXFixationDataStream(FixationDataMode mode)
    {
        _mode = mode;
        Last = EyeXFixationPoint.Invalid;
    }

    public override string Id
    {
        get { return string.Format("EyeXFixationDataStream/{0}", _mode); }
    }

    protected override void AssignBehavior(Interactor interactor)
    {
        var behaviorParams = new FixationDataParams() { FixationDataMode = _mode };
        interactor.CreateFixationDataBehavior(ref behaviorParams);
    }

    public override void HandleEvent(InteractionEvent event_, Vector2 gameWindowPosition, float horizontalScreenScale, float verticalScreenScale)
    {
        // Note that this method is called on a worker thread, so we MAY NOT access any game objects from here.
        // The data is stored in the Last property and used from the main thread.
        foreach (var behavior in event_.Behaviors)
        {
            if (behavior.BehaviorType != BehaviorType.FixationData) { continue; }

            FixationDataEventParams fixationEventParams;
            if (behavior.TryGetFixationDataEventParams(out fixationEventParams))
            {
                var gazePoint = new EyeXGazePoint(
                    (float)fixationEventParams.X, 
                    (float)fixationEventParams.Y, 
                    fixationEventParams.Timestamp,
                    gameWindowPosition,
                    horizontalScreenScale,
                    verticalScreenScale);
                Last = new EyeXFixationPoint(gazePoint, fixationEventParams.Timestamp, fixationEventParams.EventType);
            }
        }
    }

    protected override void OnStreamingStarted()
    {
        Last = EyeXFixationPoint.Invalid;
    }
}
