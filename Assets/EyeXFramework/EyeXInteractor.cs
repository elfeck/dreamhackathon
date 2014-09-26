﻿//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;
using UnityEngine;
using Rect = UnityEngine.Rect;

/// <summary>
/// Represents an EyeX interactor in a Unity game/application. Used with the EyeX host.
/// </summary>
public class EyeXInteractor
{
    private string _id;
    private string _parentId;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="interactorId">Interactor ID.</param>
    /// <param name="parentId">Parent interactor ID.</param>
    public EyeXInteractor(string interactorId, string parentId)
    {
        _id = interactorId;
        _parentId = parentId;
        EyeXBehaviors = new List<IEyeXBehavior>();
    }

    /// <summary>
    /// Gets the ID of the interactor.
    /// </summary>
    public string Id
    {
        get { return _id; }
    }

    /// <summary>
    /// Gets or sets the location of the interactor.
    /// </summary>
    public ProjectedRect Location { get; set; }

    /// <summary>
    /// Gets or sets the stencil mask for the interactor.
    /// </summary>
    public EyeXMask Mask { get; set; }

    /// <summary>
    /// Gets or sets the EyeX behaviors assigned to the interactor.
    /// See for example <see cref="EyeXActivatable"/> and <see cref="EyeXGazeAware"/>.
    /// </summary>
    public IList<IEyeXBehavior> EyeXBehaviors { get; set; }

    /// <summary>
    /// Adds the interactor to the given snapshot.
    /// </summary>
    /// <param name="snapshot">Interaction snapshot.</param>
    /// <param name="windowId">ID of the game window.</param>
    /// <param name="gameWindowPosition">Position of the game window in screen coordinates.</param>
    public void AddToSnapshot(Snapshot snapshot, string windowId, Vector2 gameWindowPosition, float horizontalScreenScale, float verticalScreenScale)
    {
        var interactor = snapshot.CreateInteractor(_id, _parentId, windowId);

        var bounds = interactor.CreateBounds(BoundsType.Rectangular);
        bounds.SetRectangularData(
            (Location.rect.x + gameWindowPosition.x) / horizontalScreenScale,
            (Location.rect.y + gameWindowPosition.y) / verticalScreenScale,
            Location.rect.width / horizontalScreenScale,
            Location.rect.height / horizontalScreenScale);

        interactor.Z = Location.relativeZ;

        if (Mask != null &&
            Mask.Type != EyeXMaskType.None)
        {
            interactor.CreateMask(MaskType.Default, Mask.Size, Mask.Size, Mask.MaskData);
        }

        foreach (var behavior in EyeXBehaviors)
        {
            behavior.AddTo(interactor);
        }
    }

    /// <summary>
    /// Invokes the event handler function.
    /// </summary>
    /// <param name="event_">Event object.</param>
    public void HandleEvent(InteractionEvent event_)
    {
        foreach (var behavior in EyeXBehaviors)
        {
            behavior.HandleEvent(event_);
        }
    }

    /// <summary>
    /// Tells whether the bounds of an interactor intersects with a given rectangle.
    /// </summary>
    /// <param name="rectangle">Bounds in GUI coordinates.</param>
    /// <returns>True if the interactor bounds and the rectangle intersect.</returns>
    public bool IntersectsWith(Rect rectangle)
    {
        return Location.isValid &&
            rectangle.Overlaps(Location.rect);
    }
}
