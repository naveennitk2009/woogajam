using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Generate VS 11 compatible solution and project
/// </summary>
[RegisterSymbol(Name = "AS_2GR")]
public class MenuAssetsSyncVS11 : MenuAssetsSyncVSCommon
{
    /// <summary>
    /// Sync Unity assets into VS 11
    /// </summary>
    [MenuItem("Assets/Sync Visual Studio 11")]
    private static void SyncVisualStudio11()
    {
        try
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            string currentFolderName = currentDirectory.Name;

            string csprojGUID = m_csprojGUID;
            string slnGUID = m_slnGUID;

            WriteSolutionVisualStudio11(slnGUID, csprojGUID);
            WriteProjectVisualStudio11(csprojGUID);

            // get the reload dialog to appear when a file is added
            string fileSuo = string.Format("{0}{1}", currentFolderName, FILE_SUO_11_SUFFIX);
            FileInfo fi = new FileInfo(fileSuo);
            if (fi.Exists)
            {
                fi.CreationTime = DateTime.Now;
                fi.LastWriteTime = DateTime.Now;
            }

            string fileCSProj = string.Format("{0}{1}", currentFolderName, FILE_CSPROJ_11_SUFFIX);
            fi = new FileInfo(fileCSProj);
            if (fi.Exists)
            {
                fi.CreationTime = DateTime.Now;
                fi.LastWriteTime = DateTime.Now;
            }

            Debug.Log(string.Format("{0} Sync Complete", System.DateTime.Now));
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Failed to sync VS11 exception={0}", ex));
        }
    }

    /// <summary>
    /// Check if we should display the menu item
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/Sync Visual Studio 11", validate = true)]
    public static bool CheckSyncVS()
    {
        return PlayerPrefs.HasKey(ToolboxEditor.PLAYER_PREFS_KEY_PATH_UNITY);
    }

    private static void WriteSolutionVisualStudio11(string slnGUID, string csprojGUID)
    {
        try
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            string currentFolderName = currentDirectory.Name;

            string projectSection = TEMPLATE_SOLUTION_PROJECT;
            projectSection = projectSection.Replace("__CSPROJ_GUID__", csprojGUID);
            projectSection = projectSection.Replace("__SLN_GUID__", slnGUID);
            string fileCSProj = string.Format("{0}{1}", currentFolderName, FILE_CSPROJ_11_SUFFIX);
            projectSection = projectSection.Replace("__CSPROJ_11__", currentFolderName);
            projectSection = projectSection.Replace("__FILE_CSPROJ_11__", fileCSProj);

            string globalSection = TEMPLATE_SOLUTION_GLOBAL_SECTION;
            globalSection = globalSection.Replace("__CSPROJ_GUID__", csprojGUID);

            string sln = TEMPLATE_SOLUTION;
            sln = sln.Replace("__TEMPLATE_SOLUTION_PROJECT__", projectSection);
            sln = sln.Replace("__TEMPLATE_SOLUTION_GLOBAL_SECTION__", globalSection);

            string fileSln = string.Format("{0}{1}", currentFolderName, FILE_SLN_11_SUFFIX);
            using (StreamWriter sw = new StreamWriter(fileSln))
            {
                sw.Write(sln);
                sw.Flush();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Failed WriteSolutionVisualStudio11 exception={0}", ex));
        }
    }

    private const string FILE_SLN_11_SUFFIX = ".Unity.sln";
    private const string FILE_CSPROJ_11_SUFFIX = ".Unity.csproj";
    private const string FILE_SUO_11_SUFFIX = ".Unity.suo";
    private const string FILE_CSPROJ_USER_11_SUFFIX = ".Unity.csproj.user";

    private const string TEMPLATE_SOLUTION_PROJECT =
        @"Project(""{__SLN_GUID__}"") = ""__CSPROJ_11__"", ""__FILE_CSPROJ_11__"", ""{__CSPROJ_GUID__}""
EndProject";

    private const string TEMPLATE_SOLUTION_GLOBAL_SECTION =
        @"		{__CSPROJ_GUID__}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{__CSPROJ_GUID__}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{__CSPROJ_GUID__}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{__CSPROJ_GUID__}.Release|Any CPU.Build.0 = Release|Any CPU";

    /// <summary>
    /// The template of the entire solution file
    /// </summary>
    private const string TEMPLATE_SOLUTION =
        @"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 11

__TEMPLATE_SOLUTION_PROJECT__
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
__TEMPLATE_SOLUTION_GLOBAL_SECTION__
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection

EndGlobal";

    private static void WriteProjectVisualStudio11(string csprojGUID)
    {
        try
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            string currentFolderName = currentDirectory.Name;

            Dictionary<string, string> compileFiles = new Dictionary<string, string>();
            foreach (string extension in EXTENSIONS_COMPILE)
            {
                GetAssets(extension, compileFiles);
            }

            Dictionary<string, string> includeFiles = new Dictionary<string, string>();
            foreach (string extension in EXTENSIONS_INCLUDE)
            {
                GetAssets(extension, includeFiles);
            }

            Dictionary<string, string> dllFiles = new Dictionary<string, string>();
            GetAssets("*.dll", dllFiles);

            StringBuilder compiles = new StringBuilder();
            StringBuilder includes = new StringBuilder();
            StringBuilder references = new StringBuilder();

            DirectoryInfo assets = new DirectoryInfo("Assets");
            Uri assetsUri = new Uri(assets.FullName);

            foreach (KeyValuePair<string, string> kvp in compileFiles)
            {
                if (string.IsNullOrEmpty(kvp.Key) ||
                    string.IsNullOrEmpty(kvp.Value))
                {
                    continue;
                }

                string relativePath = assetsUri.MakeRelativeUri(new Uri(kvp.Value)).ToString();
                compiles.Append(TEMPLATE_PROJECT_COMPILE.Replace("__FILE_RELATIVE_PATH__",
                                                                 relativePath.Replace(@"/", @"\").Replace("&", "&amp;")));
                compiles.AppendLine();
            }

            foreach (KeyValuePair<string, string> kvp in includeFiles)
            {
                if (string.IsNullOrEmpty(kvp.Key) ||
                    string.IsNullOrEmpty(kvp.Value))
                {
                    continue;
                }

                string relativePath = assetsUri.MakeRelativeUri(new Uri(kvp.Value)).ToString();
                compiles.Append(TEMPLATE_PROJECT_INCLUDE.Replace("__FILE_RELATIVE_PATH__",
                                                                 relativePath.Replace(@"/", @"\").Replace("&", "&amp;")));
                compiles.AppendLine();
            }

            foreach (KeyValuePair<string, string> kvp in dllFiles)
            {
                if (string.IsNullOrEmpty(kvp.Key) ||
                    string.IsNullOrEmpty(kvp.Value))
                {
                    continue;
                }

                string relativePath = assetsUri.MakeRelativeUri(new Uri(kvp.Value)).ToString();
                string hint = TEMPLATE_PROJECT_HINT.Replace("__FILE_RELATIVE_PATH__",
                                                            relativePath.Replace(@"/", @"\").Replace("&", "&amp;"));
                string reference = TEMPLATE_PROJECT_REFERENCE;
                string fileName = Path.GetFileNameWithoutExtension(kvp.Value);
                reference = reference.Replace("__FILE_NAME__", fileName);
                reference = reference.Replace("__TEMPLATE_PROJECT_HINT__", hint);
                references.Append(reference);
                references.AppendLine();
            }

            string csproj = TEMPLATE_PROJECT_11;
            csproj = csproj.Replace("__CSPROJ_GUID__", csprojGUID);

            csproj = csproj.Replace("__DEFINE_CONSTANTS__", GetAllDefinedConstants());

            string unityEditorPath = GetUnityEditorPath();
            csproj = csproj.Replace("__PATH_UNITY_EDITOR__", unityEditorPath);

            csproj = csproj.Replace("__TEMPLATE_PROJECT_COMPILES__", compiles.ToString());
            csproj = csproj.Replace("__TEMPLATE_PROJECT_INCLUDES__", includes.ToString());

            string fileCSProj = string.Format("{0}{1}", currentFolderName, FILE_CSPROJ_11_SUFFIX);
            csproj = csproj.Replace("__FILE_CSPROJ_11__", fileCSProj);
            csproj = csproj.Replace("__TEMPLATE_PROJECT_REFERENCES__", references.ToString());

            using (StreamWriter sw = new StreamWriter(fileCSProj))
            {
                sw.Write(csproj);
                sw.Flush();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError(string.Format("Failed WriteProjectVisualStudio11 exception={0}", ex));
        }
    }

    private const string TEMPLATE_PROJECT_COMPILE =
        @"     <Compile Include=""__FILE_RELATIVE_PATH__"" />";

    private const string TEMPLATE_PROJECT_INCLUDE =
        @"     <None Include=""__FILE_RELATIVE_PATH__"" />";

    private const string TEMPLATE_PROJECT_REFERENCE =
        @" <Reference Include=""__FILE_NAME__"">
__TEMPLATE_PROJECT_HINT__
 </Reference>";

    private const string TEMPLATE_PROJECT_HINT =
        @" <HintPath>__FILE_RELATIVE_PATH__</HintPath>";

    private const string TEMPLATE_PROJECT_11 =
        @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
	<Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
	<Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
	<ProductVersion>10.0.20506</ProductVersion>
	<SchemaVersion>2.0</SchemaVersion>
	<ProjectGuid>{__CSPROJ_GUID__}</ProjectGuid>
	<OutputType>Library</OutputType>
	<AppDesignerFolder>Properties</AppDesignerFolder>
	<RootNamespace></RootNamespace>
	<AssemblyName>Assembly-CSharp</AssemblyName>
	<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>
	<FileAlignment>512</FileAlignment>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
	<DebugSymbols>true</DebugSymbols>
	<DebugType>full</DebugType>
	<Optimize>false</Optimize>
	<OutputPath>Temp\bin\Debug\</OutputPath>
	<DefineConstants>__DEFINE_CONSTANTS__</DefineConstants>
	<ErrorReport>prompt</ErrorReport>
	<WarningLevel>4</WarningLevel>
	<NoWarn>0169</NoWarn>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
	<DebugType>pdbonly</DebugType>
	<Optimize>true</Optimize>
	<OutputPath>Temp\bin\Release\</OutputPath>
	<DefineConstants>TRACE</DefineConstants>
	<ErrorReport>prompt</ErrorReport>
	<WarningLevel>4</WarningLevel>
	<NoWarn>0169</NoWarn>
  </PropertyGroup>
  <ItemGroup>
	<Reference Include=""System"" />
    <Reference Include=""System.XML"" />
	<Reference Include=""System.Core"" />
	<Reference Include=""UnityEngine"">
	  <HintPath>__PATH_UNITY_EDITOR__\UnityEngine.dll</HintPath>
	</Reference>
	<Reference Include=""UnityEditor"">
	  <HintPath>__PATH_UNITY_EDITOR__\UnityEditor.dll</HintPath>
	</Reference>
  </ItemGroup>
  <ItemGroup>
__TEMPLATE_PROJECT_COMPILES__
__TEMPLATE_PROJECT_INCLUDES__
__TEMPLATE_PROJECT_REFERENCES__
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
	   Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name=""BeforeBuild"">
  </Target>
  <Target Name=""AfterBuild"">
  </Target>
  -->
  
</Project>";
}