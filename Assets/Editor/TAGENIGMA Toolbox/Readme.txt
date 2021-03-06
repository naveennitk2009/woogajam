TAGENIGMA Toolbox
--------------------------------------


Authors
-------
Tim Graupmann


Audience
--------

Package contents are targeted towards programmers wanting to extend the Unity IDE.


Compatibility
-------------

This project is targeted for Unity 3.3 or better.


What is in the toolbox?
----------------------------

Extensions! Extensions can be accessed via the menu items.


"Window/Open TAGENIGMA Toolbox" - Opens a configuration panel for the toolbox. Here you can set the path to the unity editor.

On Windows, this location should be the folder containing the Unity Editor application, so that the Unity libraries can be

referenced. You can use the default install folder, or browse to the location, or search the registry for the location.

On MacOS, you could potentially be running Visual Studio in a virtual machine on the Mac in which case you still want to

export Visual Studio projects. You can browse or manually enter the path to the Unity Editor as referenced from the local

virtual machine path. In most cases the default location should be adequate.


"Assets/Sync Visual Studio 2008" - Direct exporter to Visual Studio 2008.



"Assets/Sync Visual Studio 2010" - Whether you are on PC or MAC, you can now sync a VS 2010 compatible project and solution.

The export process checks the registry to find UnityEditor.dll and UnityEngine.dll or defaults to using the default path.

The project is intended to reference C# script, text assets, materials, shaders, and shader include files.


"Assets/Sync Visual Studio 11" - Direct exporter to Visual Studio 2011.


"Assets/Remove C# Bak and C# Bak Meta Files" - Using external source control tools can create backup files. Unity automatically

generates meta files for your backup files. And then you can run into compile issues. This script will remove the autogenerated

backup files and redundant meta files.


"Assets/Convert to Dos Line Endings" - Mix-matched line endings can cause compile warnings. This will standardize the line

endings in your C# scripts to DOS style line endings.


"Assets/Convert to Unix Endings" - Mix-matched line endings can cause compile warnings. This will standardize the line

endings in your C# scripts to Unix style line endings.


Register Symbol - Pre-processor defines can now be automatically exported to the project.  Simply add a custom attribute

to your MonoBehaviour script and it will be detected and exported. The example format is as follows:

[RegisterSymbol(Name = "AS_2GR")]
public class YourClassName : MonoBehaviour


Q & A
-----

You can send comments/questions to tim@tagenigma.com and please rate this package.