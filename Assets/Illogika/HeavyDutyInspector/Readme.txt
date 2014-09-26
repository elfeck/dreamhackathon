Heavy-Duty Inspector
====================
Copyright © 2013 - 2014  Illogika

Contact us at support@illogika.com

Heavy-Duty Inspector addresses the default Inspector’s shortcomings with easy-to-use
property attributes for your scripts.

Installation Instructions
=========================

Although importing this package in your project is enough to use all of its features, we
have provided for your convenience a script template to create a NamedMonoBeahviour. To
install it, simply use the Install Script Templates command under the File menu. This will
add all text files found under the Assets/Illogika/ScriptTemplates folder to the currently
running installation of Unity.

General Instructions
====================

To use any of these property attributes, simply add it before the variable you want to
modify. Note that because of the way Unity's Property Drawers work, you can only use one
of these attributes per variable. It is also important to note that some of these
attributes (like Comment or Button) don't really change the way the variable is displayed,
but rather add something before it. It is done like this because property attributes have
to be aplied to a variable, and the only other way to do it would be to write a custom
Inspector. Whenever this is the case, you will have the option to hide the modified
variable.

A note on Decorator Drawers
===========================

Unity 4.5 introduced Decorator Drawers. Decorator Drawers differ from Property Drawers in
the sense that they don't change the way the variable they are attached to is displayed.
Instead they allow to display something else before the variable in the Inspector.

This is something that was lacking from the old Property Drawer system as some of the Attributes
like Comment or Image had to be attached to "dummy" variables to work. While this is still the
case with DecoratorDrawers, you can add several of them to the same variable, or even add them
to a variable that already has a Property Drawer. 

Since [HideInInspector] not only hides a field, but also prevents any of its DecoratorDrawers
from being called, I added a new attribute, [HideVariable] to allow attaching comments to
invisible variables in the event you absolutely need it.

Also note that, while Buttons should be DecoratorDrawers, for now they can't because DecoratorDrawers
currently don't have a reference to the MonoBehaviour they are displayed in, which prevents them
from knowing whose function to call.


NamedMonoBehaviours
===================

NamedMonoBehaviours were created to help identify script references within an object. We
have all ended up with an object full of instances of the same script, especially when
designing a state machine, and the Inspector displays these script references in a way
that is not helpful. You end up with countless variables, all with the same GameObject
name and the same script name, until you no longer know which variable references which
script. You could make a hierarchy of empty GameObjects to sort it out, but it is wasteful
and time consuming to create and navigate through afterwards.

Using the NamedMonoBehaviour class, its Property Attribute and Property Drawer, you can
now give a name to your scripts and have it displayed next to the reference in the
Inspector.

Unity 4.0 to 4.2
----------------
[NamedMonoBehaviourAttribute(System.Type classType)]
classType: The type of the class you want to display as a NamedMonoBehaviour. It must
extend NamedMonoBehaviour to work.

To have the content of Lists and Arrays displayed as a NamedMonoBehaviour, you must extend
the NamedMonoBehaviourDrawer class. You can use the example provided as a starting point.

Unity 4.3 or later
------------------
[NamedMonoBehaviourAttribute]
In Unity 4.3 Property attributes are now properly used to display List and Array contents
and not the List or Array itself, which means you can directly use the
NamedMonoBehaviourAttribute on Lists and Arrays without going through the hassle of
extending the Drawer class.


Component Selection
===================

One of the worst features of the default Inspector is the inability to choose which
Component you want to select when the Component is on another GameObject. Also, it can
become tedious to drag and drop your Components across a full Inspector several screens
long.

With the ComponentSelection Attribute, you can display a reference to a Component as a
reference to its owning GameObject, followed by a popup box with a list of every Component
matching the given type on the selected object. Most Components will be named after their
type, and numbered, but NamedMonoBehaviours will display their full names (numbering will
be added if duplicate names exist). You can also specify the name of a field you know your
Component has and its value will be displayed in the list next to the Component.

To visualize your references even more easily, the object reference is displayed in green
if the Component is on the same GameObject as your script, or in yellow if it is on
another GameObject.

Unity 4.0 to 4.2
----------------
[ComponentSelection]
Default constructor. Will find all object of type Component.

[ComponentSelection(System.Type componentType)]
componentType:    The type of Component you want to select.

[ComponentSelection(System.Type componentType, string fieldName)]
componentType:    The type of Component you want to select.
fieldName:        The name of a variable present in the Component whose value you want
displayed next to the Component's name and numbering.

Because of a limitation in the way Property Drawers work, if you have a List or Array of
custom serializable objects that contain variables using ComponentSelection, all variables
with the same name will point to the same GameObject. This is because Unity creates a
single instance of Property Drawer for all of them and there was no way before Unity 4.3
came out to know which item in the list we are currently editing.

Unity 4.3 or later
------------------
[ComponentSelection]
Default constructor. Will find all object of the type corresponding to your variable.

[ComponentSelection(string fieldName)]
fieldName:	The name of a variable present in the Component whose value you want displayed
next to the Component's name and numbering.

[ComponentSelection(string defaultObject, string fieldName)]
defaultObject:	The name of a GameObject in your scene that will be selected by default.
fieldName:	The name of a variable present in the Component whose value you want displayed
next to the Component's name and numbering.

ReorderableList (Unity 4.3+ only)
=================================
[ReorderableList(bool doubleComponentRefSizeInChildren, bool useNamedMonoBehaviourDrawer = false)]
doubleComponentRefSizeInChildren: whether or not you want Reorderable List to double the
height of Component and NamedMonoBehaviour references in children. Use if you plan on
using ComponentSelection and NamedMonoBehaviour attributes inside a custom serialized
object.
useNamedMonoBehaviourDrawer: if your list is a list of NamedMonoBehaviour, whether or not
you want to use the basic NamedMonoBehaviourAttribute UI to display the list's elements,
instead of the ComponentSelectionAttribute UI.

Another flaw of the Inspector is that it does not take advantage of the full power of
Lists. Both Arrays and Lists are displayed in the exact same way in the Inspector. With
the ReorderableList attribute, you can move elements up and down the List, and even add or
remove elements from anywhere in the List, not just at the end. It not only supports lists
of custom serializable objects, but displays Component references using the
ComponentSelectionAttribute UI, and has an option to use the basic
NamedMonoBehaviourAttribute UI instead if your list is a list of NamedMonoBehaviour.


Other Property Attributes
=========================

AssetPaths
----------
[AssetPath(PathOptions pathOptions)]
pathOptions:	The way your path should be formatted. Relative to the Assets folder, 
relative to a Resources folder and with no extension, or just the filename.

[AssetPath(System.Type type, PathOptions pathOptions)]
type:   		The type of asset that can be dragged into the reference box.
pathOptions:	The way your path should be formatted. Relative to the Assets folder, 
relative to a Resources folder and with no extension, or just the filename.

Display a string as an object reference in the Inspector. Never have to type your assets'
paths again.


Background
----------
[Background(ColorEnum color)]
[Background(float r, float g, float b)]

Adds a solid color background to the affected variable. Useful to make Serializable Objects
appear as a foldable section Header.

Backgrounds are always displayed last, after all other DecoratorDrawers to make sure they
are applied to the variable, not another DecoratorDrawer.


Buttons
-------
[Button(string buttonText, string functionName, bool hideVariable = false)]
buttonText:    The text displayed on the button.
functionName:    The name of the function to call.
hideVariable:    Whether or not you want to display the variable this attribute is applied
to. Versions of Unity before 4.3 lack an implementation for the default drawer which
prevents this attribute from displaying variables of types other than bool, int, float,
string, Color, Object reference, Rect, Vector2 and Vector3. Other types will be hidden by
default. Unity version 4.3 and over can display any variable type.

Add a button with a function callable by reflection. Pass the function's name to the
constructor as a string.

You should not use this attribute with Arrays or Lists.


ImageButtons
------------
[ImageButton(string imagePath, string functionName, bool hideVariable = false)]
imagePath:	Path to your image. This path must be relative to your Assets folder and
include the file's extension.
functionName:    The name of the function to call.
hideVariable:    Whether or not you want to display the variable this attribute is applied
to. Versions of Unity before 4.3 lack an implementation for the default drawer which
prevents this attribute from displaying variables of types other than bool, int, float,
string, Color, Object reference, Rect, Vector2 and Vector3. Other types will be hidden by
default. Unity version 4.3 and over can display any variable type.

Add a button with a function callable by reflection. Pass the function's name to the
constructor as a string.

You should not use this attribute with Arrays or Lists.


ChangeCheckCallback
-------------------
[ChangeCheckCallback(string callbackName)]
callbackName:	The name of the function to call when the value of the varialbe changes.

Use this attribute to call a function when the value of its variable changes.

This attribute needs to start a coroutine to work correctly and will not work on classes
extending ScriptableObjects as they lack the ability to use a coroutine.

Versions of Unity before 4.3 lack an implementation for the default drawer which
prevents this attribute from displaying variables of types other than bool, int, float,
string, Color, Object reference, Rect, Vector2 and Vector3. Other types will not appear
in the Inspector. Unity version 4.3 and over can display any variable type.


Comments
--------
[Comment(string comment, CommentType messageType = CommentType.None, int order = 0)]
comment:        The comment to be displayed.
messageType:    The icon to be displayed next to the comment. This is the same as the
Editor enum MessageType.
order:			The order in which to display DecoratorDrawers, from smallest to largest.

Add a comment to explain variables to your game designer. Code comments are not read by
designers and are sometimes ignored by programmers. Add a comment in the Inspector for
everyone to see without the hassle of writing a custom Inspector.

Comment attribute now uses a DecoratorDrawer, so it is now safe to apply it to Lists
and Arrays. You can also add it multiple times on the same variable.

ComplexHeader
-------------
[ComplexHeader(string text, Style style, Alignment textAlignment, ColorEnum textColor, ColorEnum backgroundColor)]
[ComplexHeader(string text, Style style, Alignment textAlignment, float textColorR, float textColorG, float textColorB, float backgroundColorR, float backgroundColorG, float backgroundColorB)]
text:		The text to be displayed in the header.
style:		The header style, either Box or Line.
textAlignment:	The text's alignment, either Left, Centered or Right.
textColor:	Either an enum with the standard colors or three floats for the R, G, and B values
of the Text color. This is because Attributes can only be constants and the Color class cannot
be a constant.
backgroundColor:	Either an enum with the standard colors or three floats for the R, G, and
B values of the Text color. This is because Attributes can only be constants and the Color
class cannot be a constant.

Displays a header in the inspector for easy categorization of variables. The header can
either be of style "Box", with the title displayed over a solid color background, or of
style "Line" with the title being surrounded by a line.

Headers are always displayed first, before any other DecoratorDrawer.


HideVariable
------------
[HideVariable]

Like [HideInInspector] but doesn't prevent DecoratorDrawers from being called.


Hide Conditional (Unity 4.3+ only)
----------------------------------
[HideConditional(string conditionVariableName)]
conditionVariableName:	The name of the variable to use as a condition. The condition will
be that the variable's value is not null.

[HideConditional(string conditionVariableName, bool visibleState)]
[HideConditional(string conditionVariableName, params int[] visibleState)]
conditionVariableName:	The name of the variable to use as a condition.
visibleState:	The state the condition variable needs to be in for the variable to be
displayed. This can be either a boolean or a list of integers corresponding to the values
of an int or enum.

[HideConditional(string conditionVariableName, float minValue, float maxValue)]
minValue:	The minimum value the condition variable can be for this variable to be displayed.
maxValue:	The maximum value the condition variable can be for this variable to be displayed.

Hide a variable in the Inspector until another variable has the specified value. The
condition can be that the variable is not null, that it has a specific boolean value, that
it has a specific int value among a set of int values (this can be the int value of an
enum), or has a float value between a min and a max.


Images
------
[Image(string imagePath, Alignment alignment = Alignment.Center, int order = 0)]
imagePath:	Path to your image. This path must be relative to your Assets folder and
include the file's extension.
alignment:	The image's alignment, either Left, Center or Right.
order:			The order in which to display DecoratorDrawers, from smallest to largest.

When a comment is not enough, a picture is worth a thousand words. Add a diagram to the
Inspector, or maybe just a logo that visually represents the class function.

The image attribute now uses a DecoratorDrawer, so it is now safe to apply it to Lists
and Arrays. You can also add it multiple times on the same variable.


Tags
----
[Tag]

Display a string using the Tags drop-down menu in the Inspector. It is both convenient and
prevents typos.


Tags List (Unity 4.3+ only)
---------------------------
[TagList(canDeleteFirstElement = true)]
canDeleteFirstElement:	Whether or not you want the first element of the list to be deletable.

Display a List of strings using the Tags drop-down menu in the Inspector. This also allows
you to delete tags from anywhere in the list.


Layers
------
[Layer]
Display an int using the Layers drop-down menu.

Text Area (Depreciated in Unity 4.5)
---------
[TextArea]

Displays a string using a multi line, self expanding text area instead of the default single line
the Inspector usually shows.


ChangeLog
=========

v1.11
- Added support for private or protected fields to the Drawers that were using reflection. You still
need to specify [SerializeField] for Unity to actually serialize it, or even call the Drawer.

v1.1
- Updated Heavy Duty Inspector for Unity 4.5
- Removed TextArea in Unity 4.5 as it has been added by Unity. It is still present if you are using
an older version of Unity.
- CommentAttribute and ImageAttribute now use the new DecoratorDrawer, which means you can apply
several of them to the same variable, and even add them to a variable that already uses a
Property Drawer.
- Added the HideVariable attribute, which does the same thing as HideInInspector, but without preventing
DecoratorDrawers from being called.
- Added the ComplexHeader attribute to display headers in the inspector that catch the eye better than
the basic HeaderAttribute introduced in Unity 4.5
- Added a Background attribute to add a solid color background to a variable.

v1.06
- Fixed a bug where Component Selection and Reorderable list would not serialize their object when
changing the target object for component selection, or when reordering elements in the list without
making any changes to the value of these elements.

v1.05
- Added support for classes extending ScriptableObject.
- ComponentSelection now selects the first component matching the type by default when the target
object has changed instead of keeping a null value.

v1.04
- Moved all Attributes and Drawers to the HeavyDutyInspector namespace to prevent name clashing
with other plugins.
- added "using HeavyDutyInspector;" to the script template.

v1.03
- Added the TextArea attribute.
- Fixed a bug with ImageButtons not displaying their attached variable correctly.
- Fixed a bug with height calculation for Rect types

Known Issues:
- There appears to be a bug with the EditorGUI.PropertyField function that prevents it
from displaying Quaternions properly. This prevents Buttons, Comments and Images from
displaying Quaternions in Unity 4.3+

v1.02
- Added the ImageButton attribute. Works like the Button attribute, but takes the path to an image
instead of the button's text as a parameter.
- Added a parameter to the Image attribute to Left, Right or Center align the image.
- Fixed a bug that would stretch images horizontally.

v1.01		Initial Release
