using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RegisterSymbol(Name = "AS_2GR")]
public class ToolboxEditor : EditorWindow
{
    /// <summary>
    /// Open an instance of the panel
    /// </summary>
    /// <returns></returns>
    public static ToolboxEditor GetPanel()
    {
        ToolboxEditor window = GetWindow<ToolboxEditor>();
        window.position = new Rect(300, 300, 500, 500);
        return window;
    }

    /// <summary>
    /// Get Toolbox Window
    /// </summary>
    [MenuItem("Window/Open TAGENIGMA Toolbox")]
    private static void MenuWindowGetToolbox()
    {
        GetPanel();
    }

    /// <summary>
    /// The scroll vector
    /// </summary>
    private Vector2 m_scroll = Vector2.zero;

    /// <summary>
    /// The registry key that holds the location of the unity editor
    /// </summary>
    private const string REGISTRY_UNITY_EDITOR_LOCATION =
#if UNITY_4
        @"Software\Unity Technologies\Unity Editor 4.x\Location";
#else
        @"Software\Unity Technologies\Unity Editor 3.x\Location";
#endif

    /// <summary>
    /// Default install location for the unity editor
    /// </summary>
    private const string PATH_UNITY_EDITOR_DEFAULT =
        @"C:\Program Files\Unity\Editor";

    /// <summary>
    /// Default install location for the unity editor (86)
    /// </summary>
    private const string PATH_UNITY_EDITOR_DEFAULT_X86 =
        @"C:\Program Files (x86)\Unity\Editor";

    /// <summary>
    /// Get the path to the unity editor from the registry
    /// </summary>
    /// <returns></returns>
    private static string GetPathRegistryUnityEditor()
    {
        string unityEditorPath = string.Empty;
        using (
            Microsoft.Win32.RegistryKey key =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_UNITY_EDITOR_LOCATION, false))
        {
            if (null == key)
            {
                Debug.LogWarning(string.Format("Registry Unity Editor Location was not found: {0}",
                                               REGISTRY_UNITY_EDITOR_LOCATION));
            }
            else
            {
                object obj = key.GetValue(string.Empty);
                if (null == obj)
                {
                    Debug.LogWarning(string.Format("Registry Unity Editor Location value was not found: {0}",
                                                   REGISTRY_UNITY_EDITOR_LOCATION));
                }
                if (null != obj)
                {
                    string editorPath = obj.ToString();
                    FileInfo fi = new FileInfo(editorPath);
                    if (fi.Exists)
                    {
                        unityEditorPath = fi.Directory.FullName.Replace("\\", "/");
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("Registry Unity Editor was not found: {0} path={1}",
                                                       REGISTRY_UNITY_EDITOR_LOCATION, editorPath));
                    }
                }
            }
        }
        if (string.IsNullOrEmpty(unityEditorPath))
        {
            Debug.LogWarning("Could not read Unity Editor Directory Path from the registry using default");
            unityEditorPath = PATH_UNITY_EDITOR_DEFAULT;
        }

        return unityEditorPath;
    }

    string GetPathProcessUnityEditor()
    {
        string result = string.Empty;
        try
        {
            Process process = Process.GetCurrentProcess();  // Process.GetProcesses(); if you dont have.
            if (null != process)
            {
                FileInfo fi = new FileInfo(process.Modules[0].FileName);
                if (fi.Exists)
                {
                    return fi.Directory.FullName;
                }
            }

        }
        catch (Exception)
        {
        }
        return result;
    }

    /// <summary>
    /// Playerprefs key
    /// </summary>
    public const string PLAYER_PREFS_KEY_PATH_UNITY = "PathToUnity";

    /// <summary>
    /// Player prefs key
    /// </summary>
    public const string PLAYER_PREFS_KEY_LINE_ENDINGS = "LineEndings";

    /// <summary>
    /// Line ending modes
    /// </summary>
    public enum LineModes
    {
        Dos,
        Unix,
    }

    /// <summary>
    /// The current line ending mode
    /// </summary>
    public LineModes m_lineMode = LineModes.Dos;

    /// <summary>
    /// Get the line mode from player prefs
    /// </summary>
    public static LineModes GetLineMode()
    {
        try
        {
            if (PlayerPrefs.HasKey(PLAYER_PREFS_KEY_LINE_ENDINGS))
            {
                string val = PlayerPrefs.GetString(PLAYER_PREFS_KEY_LINE_ENDINGS);
                return (LineModes)Enum.Parse(typeof(LineModes), val);
            }
        }
        catch (System.Exception)
        {
        }
        return LineModes.Dos;
    }

    /// <summary>
    /// The display code
    /// </summary>
    private void OnGUI()
    {
        GUILayout.BeginScrollView(m_scroll);

        GUILayout.Label("The exporters needs a path to the Unity Editor folder.");
        GUILayout.Label("This path is used in the exported solution to reference");
        GUILayout.Label("the unity editor and unity engine libraries.");

        GUILayout.Space(5);

        GUILayout.Label("MacOS doesn't have a registry so you can either");
        GUILayout.Label("use the default or type in the path.");

        GUILayout.Space(5);

        string oldUnityPath = PlayerPrefs.GetString(PLAYER_PREFS_KEY_PATH_UNITY);
        string newUnityPath = oldUnityPath;
        if (string.IsNullOrEmpty(oldUnityPath))
        {
            oldUnityPath = string.Empty;
            newUnityPath = string.Empty;
        }

        GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
        string path = EditorGUILayout.TextField("Path to unity:", newUnityPath);
        GUI.FocusControl("toggleButton"); //workaround to dirty GUI

        if (string.IsNullOrEmpty(path))
        {
            newUnityPath = string.Empty;
        }
        else
        {
            newUnityPath = path;
        }

        GUILayout.Label("Get the path to the Unity Editor from the Unity process.");
        GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
        if (GUILayout.Button("Use Process"))
        {
            GUI.FocusControl("toggleButton"); //workaround to dirty GUI
            newUnityPath = GetPathProcessUnityEditor();
        }

        GUILayout.Space(5);

        GUILayout.Label("Navigate to the parent folder.");
        GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
        if (GUILayout.Button("Parent Folder"))
        {
            GUI.FocusControl("toggleButton"); //workaround to dirty GUI
            DirectoryInfo parent = new DirectoryInfo((oldUnityPath));
            if (null != parent.Parent)
            {
                newUnityPath = parent.Parent.FullName.Replace("\\", "/");
            }
        }

        GUILayout.Space(5);

        GUILayout.Label("Use the default 64-bit install path:");

        GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
        if (GUILayout.Button("Use Default"))
        {
            GUI.FocusControl("toggleButton"); //workaround to dirty GUI
            newUnityPath = PATH_UNITY_EDITOR_DEFAULT;
        }

        GUILayout.Space(5);

        GUILayout.Label("Use the default 32-bit install path:");

        GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
        if (GUILayout.Button("Use Default (x86)"))
        {
            GUI.FocusControl("toggleButton"); //workaround to dirty GUI
            newUnityPath = PATH_UNITY_EDITOR_DEFAULT_X86;
        }

        GUILayout.Space(5);

        GUILayout.Label("Use the registry to look for the Unity Editor:");

        GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
        if (GUILayout.Button("Use Registry"))
        {
            GUI.FocusControl("toggleButton"); //workaround to dirty GUI
            newUnityPath = GetPathRegistryUnityEditor();
        }

        GUILayout.Space(5);

        GUILayout.Label("Use a file browser to look for the Unity Editor:");

        GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
        if (GUILayout.Button("Use Folder Browser"))
        {
            GUI.FocusControl("toggleButton"); //workaround to dirty GUI
            if (!string.IsNullOrEmpty(oldUnityPath))
            {
                newUnityPath = EditorUtility.OpenFolderPanel("Path to unity application", oldUnityPath,
                                                             string.Empty);
            }
            else if (Directory.Exists(PATH_UNITY_EDITOR_DEFAULT))
            {
                newUnityPath = EditorUtility.OpenFolderPanel("Path to unity application", PATH_UNITY_EDITOR_DEFAULT,
                                                             string.Empty);
            }
            else
            {
                newUnityPath = EditorUtility.OpenFolderPanel("Path to unity application", GetPathRegistryUnityEditor(),
                                                             string.Empty);
            }
        }

        GUILayout.Space(5);

        if (!string.IsNullOrEmpty(newUnityPath) &&
            !oldUnityPath.Equals(newUnityPath))
        {
            PlayerPrefs.SetString(PLAYER_PREFS_KEY_PATH_UNITY, newUnityPath);
        }

        m_lineMode = GetLineMode();

        GUILayout.Label("Pick your style of line endings:");
        LineModes newLineMode = (LineModes)EditorGUILayout.EnumPopup(m_lineMode);
        if (newLineMode != m_lineMode)
        {
            m_lineMode = newLineMode;
            PlayerPrefs.SetString(PLAYER_PREFS_KEY_LINE_ENDINGS, m_lineMode.ToString());
        }

        if (GUILayout.Button("Convert Line-Endings"))
        {
            switch (m_lineMode)
            {
                case LineModes.Dos:
                    MenuAssetsConvertToDosLineEndings.AssetsConvertLineEndings();
                    break;
                case LineModes.Unix:
                    MenuAssetsConvertToUnixEndings.AssetsConvertLineEndings();
                    break;
            }
        }

        GUILayout.Space(5);

        GUILayout.Label("Custom Defined Symbols:");

        GUI.enabled = false;
        GUILayout.TextArea(MenuAssetsSyncVSCommon.GetCustomDefineConstants());
        GUI.enabled = true;

        GUILayout.Space(5);

        GUILayout.Label("Defined Symbols:");

        GUI.enabled = false;
        GUILayout.TextArea(MenuAssetsSyncVSCommon.GetDefineConstants());
        GUI.enabled = true;

        GUILayout.Space(5);

        GUILayout.EndScrollView();
    }

    void Update()
    {
        Repaint();
    }
}