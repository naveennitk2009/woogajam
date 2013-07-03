using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Convert source files to dos line endings
/// </summary>
[RegisterSymbol(Name = "AS_2GR")]
public class MenuAssetsConvertToDosLineEndings : MonoBehaviour
{
    /// <summary>
    /// Add the menu item
    /// </summary>
    [MenuItem("Assets/Convert to Dos Line Endings")]
    public static void AssetsConvertLineEndings()
    {
        try
        {
            DirectoryInfo projectDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            if (null == projectDir)
            {
                return;
            }
            foreach (DirectoryInfo subDir in projectDir.GetDirectories())
            {
                if (null == subDir)
                {
                    continue;
                }
                if (subDir.Name.ToUpper().Equals("ASSETS"))
                {
                    AssetsConvertLineEndings(subDir);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Exception={0}", ex));
        }

        AssetDatabase.Refresh();
        Debug.Log(string.Format("{0} Processing is done", DateTime.Now));
    }

    /// <summary>
    /// Check if we should display the menu item
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/Convert to Dos Line Endings", validate = true)]
    public static bool CheckConvertLineEndings()
    {
        return ToolboxEditor.GetLineMode() == ToolboxEditor.LineModes.Dos;
    }

    /// <summary>
    /// The extensions to affect
    /// </summary>
    private static string[] EXTENSION_SOURCE =
        {
            "*.cs",
            "*.js",
            "*.shader"
        };

    /// <summary>
    /// Recursively convert all source files and subfolders
    /// </summary>
    /// <param name="directory"></param>
    public static void AssetsConvertLineEndings(DirectoryInfo directory)
    {
        if (null == directory)
        {
            return;
        }
        // get all source files
        foreach (string extension in EXTENSION_SOURCE)
        {
            foreach (FileInfo file in directory.GetFiles(extension))
            {
                string content = string.Empty;
                string oldContent = string.Empty;
                // convert the lines and compare for change
                using (StreamReader sr = new StreamReader(file.FullName))
                {
                    oldContent = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(oldContent))
                    {
                        continue;
                    }
                    content = oldContent.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                }
                if (content.Equals(oldContent))
                {
                    continue;
                }
                Debug.Log(string.Format("Modified: {0}", file.Name));
                // output the change
                using (StreamWriter sw = new StreamWriter(file.FullName))
                {
                    sw.Write(content);
                    sw.Flush();
                }
            }
        }
        // scan subfolders
        foreach (DirectoryInfo subDir in directory.GetDirectories())
        {
            if (null == subDir)
            {
                continue;
            }
            if (subDir.Name.ToUpper().Equals(".SVN"))
            {
                continue;
            }
            //Debug.Log(string.Format("Directory: {0}", subDir));
            AssetsConvertLineEndings(subDir);
        }
    }
}