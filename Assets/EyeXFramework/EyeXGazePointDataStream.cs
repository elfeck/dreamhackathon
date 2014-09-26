//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;

/// <summary>
/// Provider of gaze point data. When the provider has been started it
/// will continuously update the Last property with the latest gaze point 
/// value received from the EyeX Engine.
/// </summary>
public class EyeXGazePointDataStream : EyeXDataStreamBase<EyeXGazePoint>
{
    private readonly GazePointDataMode _mode;

    /// <summary>
    /// Creates a new instance.
    /// Note: don't create instances of this class directly. Use the <see cref="EyeXHost.GetGazePointDataProvider"/> method instead.
    /// </summary>
    /// <param name="mode">Data mode.</param>
    public EyeXGazePointDataStream(GazePointDataMode mode)
    {
        _mode = mode;
        Last = EyeXGazePoint.Invalid;
    }

    public override string Id
    {
        get { return string.Format("EyeXGazePointDataStream/{0}", _mode); }
    }

    protected override void AssignBehavior(Interactor interactor)
    {
        var behaviorParams = new GazePointDataParams() { GazePointDataMode = _mode };
        interactor.CreateGazePointDataBehavior(ref behaviorParams);
    }

    public override void HandleEvent(InteractionEvent event_, Vector2 gameWindowPosition, float horizontalScreenScale, float verticalScreenScale)
    {
        // Note that this method is called on a worker thread, so we MAY NOT access any game objects from here.
        // The data is stored in the Last property and used from the main thread.
        foreach (var behavior in event_.Behaviors)
        {
            if (behavior.BehaviorType != BehaviorType.GazePointData) { continue; }

            GazePointDataEventParams eventParams;
            if (behavior.TryGetGazePointDataEventParams(out eventParams))
            {
                Last = new EyeXGazePoint(
                    (float)eventParams.X, 
                    (float)eventParams.Y, 
                    eventParams.Timestamp,
                    gameWindowPosition, 
                    horizontalScreenScale, 
                    verticalScreenScale);
            }
        }
    }

    protected override void OnStreamingStarted()
    {
        Last = EyeXGazePoint.Invalid;
    }
}
