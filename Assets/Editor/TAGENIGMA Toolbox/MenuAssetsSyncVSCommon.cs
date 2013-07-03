using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Common code for Visual Studio Export
/// </summary>
[RegisterSymbol(Name = "AS_2GR")]
public class MenuAssetsSyncVSCommon : MonoBehaviour
{
    protected static readonly string[] EXTENSIONS_COMPILE =
        {
            "*.cs",
        };

    protected static readonly string[] EXTENSIONS_INCLUDE =
        {
            "*.cginc",
            "*.cmd",
            "*.mat",
            "*.java",
            "*.js",
            "*.shader",
            "*.txt",
            "*.xml",
        };

    protected static string m_csprojGUID = System.Guid.NewGuid().ToString();
    protected static string m_slnGUID = System.Guid.NewGuid().ToString();

    protected static string GetUnityEditorPath()
    {
        string oldUnityPath = PlayerPrefs.GetString(ToolboxEditor.PLAYER_PREFS_KEY_PATH_UNITY);
        if (string.IsNullOrEmpty(oldUnityPath))
        {
            Debug.LogError("Please use the toolbox editor to set the unity editor path.");
        }
        return oldUnityPath;
    }

    protected static void GetAssets(string extension, Dictionary<string, string> files)
    {
        try
        {
            DirectoryInfo projectDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            if (null == projectDir)
            {
                return;
            }
            if (extension.Equals("*.cmd"))
            {
                GetAssets(extension, files, projectDir);
            }
            foreach (DirectoryInfo subDir in projectDir.GetDirectories())
            {
                if (null == subDir)
                {
                    continue;
                }
                if (subDir.Name.ToUpper().Equals("ASSETS"))
                {
                    GetAssets(extension, files, subDir);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Exception={0}", ex));
        }
    }

    private static void GetAssets(string extension, Dictionary<string, string> files, DirectoryInfo directory)
    {
        if (null == directory)
        {
            return;
        }
        foreach (FileInfo file in directory.GetFiles(extension))
        {
            if (string.IsNullOrEmpty(file.FullName) ||
                files.ContainsKey(file.FullName.ToLower()))
            {
                continue;
            }
            if (file.Extension.ToLower().Equals(".dll"))
            {
                //Debug.Log(string.Format("Found DLL: {0}", file.Name));
                try
                {
                    System.Reflection.AssemblyName.GetAssemblyName(file.FullName);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }
                catch (Exception)
                {
                }
            }
            files.Add(file.FullName.ToLower(), file.FullName);
        }
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
            GetAssets(extension, files, subDir);
        }
    }

    public static string GetDefineConstants()
    {
        StringBuilder sb = new StringBuilder();
#if DEBUG
        sb.Append("DEBUG;");
#endif
#if TRACE
        sb.Append("TRACE;");
#endif
#if UNITY_WEBPLAYER
        sb.Append("UNITY_WEBPLAYER;");
#endif
#if WEBPLUG
        sb.Append("WEBPLUG;");
#endif
#if ENABLE_IMAGEEFFECTS
        sb.Append("ENABLE_IMAGEEFFECTS;");
#endif
#if ENABLE_WEBCAM
        sb.Append("ENABLE_WEBCAM;");
#endif
#if ENABLE_AUDIO_FMOD
        sb.Append("ENABLE_AUDIO_FMOD;");
#endif
#if ENABLE_NETWORK
        sb.Append("ENABLE_NETWORK;");
#endif
#if ENABLE_MONO
        sb.Append("ENABLE_MONO;");
#endif
#if ENABLE_PHYSICS
        sb.Append("ENABLE_PHYSICS;");
#endif
#if ENABLE_TERRAIN
        sb.Append("ENABLE_TERRAIN;");
#endif
#if ENABLE_CACHING
        sb.Append("ENABLE_CACHING;");
#endif
#if ENABLE_SUBSTANCE
        sb.Append("ENABLE_SUBSTANCE;");
#endif
#if ENABLE_GENERICS
        sb.Append("ENABLE_GENERICS;");
#endif
#if ENABLE_CLOTH
        sb.Append("ENABLE_CLOTH;");
#endif
#if ENABLE_MOVIES
        sb.Append("ENABLE_MOVIES;");
#endif
#if ENABLE_AUDIO
        sb.Append("ENABLE_AUDIO;");
#endif
#if ENABLE_WWW
        sb.Append("ENABLE_WWW;");
#endif
#if ENABLE_SHADOWS
        sb.Append("ENABLE_SHADOWS;");
#endif
#if ENABLE_AUDIO
        sb.Append("ENABLE_AUDIO;");
#endif
#if ENABLE_WWW
        sb.Append("ENABLE_WWW;");
#endif
#if ENABLE_SHADOWS
        sb.Append("ENABLE_SHADOWS;");
#endif
#if ENABLE_DUCK_TYPING
        sb.Append("ENABLE_DUCK_TYPING;");
#endif
#if UNITY_EDITOR
        sb.Append("UNITY_EDITOR;");
#endif
#if UNITY_STANDALONE_OSX
        sb.Append("UNITY_STANDALONE_OSX;");
#endif
#if UNITY_DASHBOARD_WIDGET
        sb.Append("UNITY_DASHBOARD_WIDGET;");
#endif
#if UNITY_STANDALONE_WIN
        sb.Append("UNITY_STANDALONE_WIN;");
#endif
#if UNITY_WEBPLAYER
        sb.Append("UNITY_WEBPLAYER;");
#endif
#if UNITY_WII
        sb.Append("UNITY_WII;");
#endif
#if UNITY_IPHONE
        sb.Append("UNITY_IPHONE;");
#endif
#if UNITY_ANDROID
        sb.Append("UNITY_ANDROID;");
#endif
#if UNITY_PS3
        sb.Append("UNITY_PS3;");
#endif
#if UNITY_XBOX360
        sb.Append("UNITY_XBOX360;");
#endif
#if UNITY_NACL
        sb.Append("UNITY_NACL;");
#endif
#if UNITY_FLASH
        sb.Append("UNITY_FLASH;");
#endif
#if UNITY_2_6
        sb.Append("UNITY_2_6;");
#endif
#if UNITY_2_6_1
        sb.Append("UNITY_2_6_1;");
#endif
#if UNITY_3_0_0
        sb.Append("UNITY_3_0_0;");
#endif
#if UNITY_3_0
        sb.Append("UNITY_3_0;");
#endif
#if UNITY_3_1
        sb.Append("UNITY_3_1;");
#endif
#if UNITY_3_2
        sb.Append("UNITY_3_2;");
#endif
#if UNITY_3_3
        sb.Append("UNITY_3_3;");
#endif
#if UNITY_3_4
        sb.Append("UNITY_3_4;");
#endif
#if UNITY_3_5
        sb.Append("UNITY_3_5;");
#endif
#if UNITY_4_0_0
        sb.Append("UNITY_4_0_0;");
#endif
#if UNITY_4_0
        sb.Append("UNITY_4_0;");
#endif
#if ENABLE_PROFILER
        sb.Append("ENABLE_PROFILER;");
#endif
#if UNITY_EDITOR
        sb.Append("UNITY_EDITOR;");
#endif
#if UNITY_TEAM_LICENSE
        sb.Append("UNITY_TEAM_LICENSE;");
#endif

        return sb.ToString();
    }

    public static string GetCustomDefineConstants()
    {
        StringBuilder sb = new StringBuilder();
        
        #region Find all symbols in assembly

        List<string> symbols = new List<string>();
        Assembly myAssembly = Assembly.GetExecutingAssembly();
        foreach (Type t in myAssembly.GetTypes())
        {
            foreach (Attribute attribute in t.GetCustomAttributes(true))
            {
                if (!(attribute is RegisterSymbol))
                {
                    continue;
                }
                string symbol = (attribute as RegisterSymbol).Name;
                if (string.IsNullOrEmpty(symbol))
                {
                    continue;
                }
                if (symbols.Contains(symbol))
                {
                    continue;
                }
                //Debug.Log(string.Format("Found symbol: {0}", symbol));
                symbols.Add(symbol);
            }
        }

        foreach (string symbol in symbols)
        {
            sb.Append(symbol);
            sb.Append(";");
        }

        #endregion


        return sb.ToString();
    }

    public static string GetAllDefinedConstants()
    {
        string symbols = GetDefineConstants();
        string customSymbols = GetCustomDefineConstants();
        return string.Format("{0}{1}",
            string.IsNullOrEmpty(symbols) ? string.Empty : symbols,
            string.IsNullOrEmpty(customSymbols) ? string.Empty : customSymbols);
    }
}