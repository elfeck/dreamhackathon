//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;

/// <summary>
/// Provides utility functions related to screen and window handling.
/// </summary>
internal class ScreenHelpers
{
    private string _windowId;
    private IntPtr _hwnd;

#if UNITY_EDITOR
    private UnityEditor.EditorWindow _gameWindow;
#endif

    internal ScreenHelpers()
    {
        _hwnd = Win32Helpers.GetForegroundWindow();
        _windowId = _hwnd.ToString();

#if UNITY_EDITOR
        _gameWindow = GetMainGameView();
#endif
    }

    /// <summary>
    /// Gets or sets the Window ID for the game window.
    /// </summary>
    public string GameWindowId
    {
        get
        {
            return _windowId;
        }

        set
        {
            int hwnd;
            if (int.TryParse(value, out hwnd))
            {
                _windowId = value;
                _hwnd = new IntPtr(hwnd);
            }
        }
    }

    /// <summary>
    /// Returns the position of the game window in screen coordinates.
    /// </summary>
    /// <returns>Position in screen coordinates.</returns>
    public Vector2 GetGameWindowPosition()
    {
#if UNITY_EDITOR
        var gameWindowPosition = _gameWindow.position;
        var heightOffset = gameWindowPosition.height - Screen.height;

        // Adjust for different aspect ratios and viewport sizes
        var viewportOffsetX = (_gameWindow.position.width - Screen.width) / 2.0f;
        var viewportOffsetY = (_gameWindow.position.height - Screen.height) / 2.0f;

        return new Vector2(gameWindowPosition.x + viewportOffsetX, gameWindowPosition.y - viewportOffsetY + heightOffset);
#else
        var windowClientPosition = new Win32Helpers.POINT();
        Win32Helpers.ClientToScreen(_hwnd, ref windowClientPosition);
        return new Vector2(windowClientPosition.x, windowClientPosition.y);
#endif
    }

    /// <summary>
    /// Returns the horizontal screen scale factor
    /// to adjust coordinates in windowed full-screen mode
    /// </summary>
    /// <returns>The horizontal screen scale factor</returns>
    public float GetHorizontalScreenScale(EyeXEngineStateValue<Tobii.EyeX.Client.Rect> eyetrackerScreenBounds)
    {
        if (Screen.fullScreen && eyetrackerScreenBounds.IsValid)
        {
            return (float)(Screen.width / eyetrackerScreenBounds.Value.Width);
        }
        else return 1.0f;
    }

    /// <summary>
    /// Returns the vertical screen scale factor
    /// to adjust coordinates in windowed full-screen mode
    /// </summary>
    /// <returns>The vertical screen scale factor</returns>
    public float GetVerticalScreenScale(EyeXEngineStateValue<Tobii.EyeX.Client.Rect> eyetrackerScreenBounds)
    {
        if (Screen.fullScreen && eyetrackerScreenBounds.IsValid)
        {
            return (float)(Screen.height / eyetrackerScreenBounds.Value.Height);
        }
        else return 1.0f;
    }

#if UNITY_EDITOR
    private static UnityEditor.EditorWindow GetMainGameView()
    {
        var unityEditorType = Type.GetType("UnityEditor.GameView,UnityEditor");
        var getMainGameViewMethod = unityEditorType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        var result = getMainGameViewMethod.Invoke(null, null);
        return (UnityEditor.EditorWindow)result;
    }
#endif
}