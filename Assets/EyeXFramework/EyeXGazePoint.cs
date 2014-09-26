﻿//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

/// <summary>
/// Holds a gaze point with a timestamp and converts to either Screen space, Viewport, or GUI space coordinates.
/// </summary>
public struct EyeXGazePoint
{
    private readonly Vector2 _gazePointInDisplaySpace;
    private readonly Vector2 _gazePointInUnityGUISpace;
    private readonly double _timestamp;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="x">X coordinate in operating system screen coordinates.</param>
    /// <param name="y">Y coordinate in operating system screen coordinates.</param>
    /// <param name="timestamp">Timestamp in milliseconds.</param>
    /// <param name="gameWindowPosition">The position of the top-left corner of the game window, in operating system coordinates.</param>
    /// <param name="horizontalScreenScale">The horizontal relationship between the Unity and operating system coordinate systems.</param>
    /// <param name="verticalScreenScale">The vertical relationship between the Unity and operating system coordinate systems.</param>
    public EyeXGazePoint(float x, float y, double timestamp, Vector2 gameWindowPosition, float horizontalScreenScale, float verticalScreenScale)
    {
        _gazePointInDisplaySpace = new Vector2(x, y);
        _gazePointInUnityGUISpace = new Vector2(
            (x - gameWindowPosition.x) * horizontalScreenScale, 
            (y - gameWindowPosition.y) * verticalScreenScale);
        _timestamp = timestamp;
    }

    /// <summary>
    /// Gets a value representing an invalid gaze point.
    /// </summary>
    public static EyeXGazePoint Invalid
    {
        get
        {
            return new EyeXGazePoint(float.NaN, float.NaN, double.NaN, Vector2.zero, 1.0f, 1.0f);
        }
    }

    /// <summary>
    /// Gets the gaze point in (Unity) screen space pixels.
    /// <para>
    /// The bottom-left of the screen/camera is (0, 0); the right-top is (pixelWidth, pixelHeight).
    /// </para>
    /// </summary>
    public Vector2 Screen
    {
        get
        {
            var point = GUI;
            point.y = UnityEngine.Screen.height - 1 - point.y;
            return point;
        }
    }

    /// <summary>
    /// Gets the gaze point in the viewport coordinate system.
    /// <para>
    /// The bottom-left of the screen/camera is (0, 0); the top-right is (1, 1).
    /// </para>
    /// </summary>
    public Vector2 Viewport
    {
        get
        {
            var point = Screen;
            point.x /= UnityEngine.Screen.width;
            point.y /= UnityEngine.Screen.height;
            return point;
        }
    }

    /// <summary>
    /// Gets the gaze point in GUI space pixels.
    /// <para>
    /// The top-left of the screen is (0, 0); the bottom-right is (pixelWidth, pixelHeight).
    /// </para>
    /// </summary>
    public Vector2 GUI
    {
        get
        {
            return _gazePointInUnityGUISpace;
        }
    }

    /// <summary>
    /// Gets the gaze point in operating system display space.
    /// <para>
    /// The top-left of the display is (DisplayX, DisplayY); the right-bottom is (DisplayWidth, DisplayHeight).
    /// </para>
    /// </summary>
    public Vector2 Display
    {
        get
        {
            return _gazePointInDisplaySpace;
        }
    }

    /// <summary>
    /// Gets the timestamp for the data point in milliseconds.
    /// <para>
    /// The timestamp can be used to uniquely identify this gaze point from a previous gaze point.
    /// </para>
    /// </summary>
    public double Timestamp
    {
        get { return _timestamp; }
    }

    /// <summary>
    /// Gets a value indicating whether the point is valid or not.
    /// </summary>
    public bool IsValid
    {
        get { return !float.IsNaN(_gazePointInUnityGUISpace.x); }
    }

    /// <summary>
    /// Gets a value indicating whether the point is within the bounds of the game window or not.
    /// </summary>
    public bool IsWithinScreenBounds
    {
        get
        {
            var screenPoint = Screen;
            return IsValid &&
                   screenPoint.x >= 0 &&
                   screenPoint.x < UnityEngine.Screen.width &&
                   screenPoint.y >= 0 &&
                   screenPoint.y < UnityEngine.Screen.height;
        }
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return string.Format("{0} {1}", _gazePointInUnityGUISpace, _timestamp);
    }
}
