

--- Description ---

The Graphical Analyzing Tool was created to help programmers visually measure, compare and analyze values that change over time.

This is useful for debugging things like:

o Physics
o Animation
o AI
o State machines
o Timers
o Math calculations

and much more.

Having values displayed graphically can drastically help the programmer quickly see relationships between values and
find problems that the old fashion step-by-step debugging method would have taken ages and been an extremely frustrating process.

All graphs can be viewed in a separate editor window during runtime and loaded back in after
the game is stopped for a detailed investigation.

The plugin works in Unity Basic as well as Pro.

There's an excellent article over at Dev.Mag that explains more about graphical debugging:
http://devmag.org.za/2011/02/09/using-graphs-to-debug-physics-ai-and-animation-effectively/


If you have any questions or problems with the plugin, please email me at:

rapidgamedev@gmail.com




--- Setup & Usage ---

Only one line of code is needed to initialize the logger:

Graph.Initialize();

Then any Unity standard variable type can be logged by the Graph.Log("name", variable) method.

There are method overloads for multiple amount of variables to the log method such as: Graph.Log(name, 29, 23)
with up to 5 variables at a time, but it is important that you allways log the same amount of variables to one graph;
otherwise it might fail when being drawn or loaded later.

Here's a list of supported variable types by the log method:
byte, int, float, bool, Vector2, Vector3, Quaternion, Color, Color32, Rect.

There is also the Graph.LogEvent("name", "message") method that allows for one-shots event messages in the graph.
Events will appear in the middle of the specified graph vertically, inside editor window as small dots.
You can use the Graph.LogEvent("name", "message", Color) overload to color-code your events differently.

Also do not forget to put Graph.Dispose(); inside a OnApplicationQuit() method in one of your
scripts to free up any resource the system might have allocated.


The Graph editor window can be opened by clicking the menu item "Window->Graph Debugger" in your Unity editor menu.
To see the graphs in action, open the exmple scene "GraphExamples" in the "Rapid/Examples" folder, then hit play."

Use the mouse to move the graphs by clicking and dragging and zoom in/out with the mouse wheel.
Hovering the mouse close to a dot on a line will show the exact values of what that variable was at the time of the log call
and events will display time of event and message text.