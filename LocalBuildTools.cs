using UnityEngine;
using UnityEditor;
using ET;
using BM;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using BuildTools.Editor;

public class LocalBuildTools
{
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder returnBuilder, int size, string fileName);

    [DllImport("kernel32")]
    private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

    private static string _path = "";
    private static FileStream _fs;

    [MenuItem("Tools/LocalBuild")]
    public static void BuildApk()
    {
        try
        {
            var tempSB = new StringBuilder(1024);
            _path = tempSB.Append(Application.dataPath).Append("\\temp.ini").ToString();
            // bool isDebug = true;
            bool isNeedClean = false;
            if (File.Exists(_path))
            {
                isNeedClean = GetIniSetting(tempSB, "buildParams", "forceclean", "0") == "1";
                var buildTask = BuildReleaseSettings.Instance.buildTaskList.GetValue()
                    .Find(a => a.BuildTaskName == GetIniSetting(tempSB, "buildParams", "buildtasktemplatename", "Android"));
                if (buildTask != null)
                {
                    if (isNeedClean) BeforeBuild(buildTask);
                    StartBuild(buildTask);
                    AfterBuilt(buildTask);
                    return;
                }

                BuildTaskSetting taskSetting = new();
                taskSetting.BuildTaskId = DateTime.Now.ToString();
                taskSetting.BuildTaskName = "Template";
                taskSetting.BuildTarget = BuildTarget.Android; //this tool only provide android build
                taskSetting.ScriptBackendType = ScriptBackendType.IL2CPP;
                taskSetting.ResBuildType = ResourceBuildType.Default;
                taskSetting.IsClearResFolder = true;
                taskSetting.IsPackeageEXE = true;
                taskSetting.IsPackeageExeRes = true;
                taskSetting.IsFullPackage = true;
                taskSetting.BuildType = BuildType.Release;
                taskSetting.BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
                taskSetting.EncryptAssets = true;
                taskSetting.SecretKey = "";
                taskSetting.versionNum = "1.0.0";
                taskSetting.codeVersionName = GetIniSetting(tempSB, "buildParams", "codeversion", "1.0.0");
                taskSetting.packetVersionName = BuildReleaseHelper.GetTimeString("yyMMddHHmm");
                taskSetting.HotUpdateEnable = false;
                taskSetting.PackerType = GetPackerType(GetIniSetting(tempSB, "buildParams", "isdebug", "Debug"));
                taskSetting.OverlayInstallType = GetOverlayInstallType(GetIniSetting(tempSB, "buildParams", "overlayinstalltype", "None"));
                taskSetting.PadType = GetPadType(GetIniSetting(tempSB, "buildParams", "padtype", "None"));
                taskSetting.LogCroppingEnable = false;
                taskSetting.VersionLog = "";
                taskSetting.BuildTime = BuildReleaseHelper.GetTimeString("yyyy-MM-dd HH:mm:ss");
                taskSetting.GcIncremental = true;
                taskSetting.IsReSetFguiBundle = false;
                taskSetting.ReleaseChannel = "GooglePay";
                var temp0 = new UserScriptableDictionary<string>();
                temp0.SetValue(GetIniSetting(tempSB, "buildParams", "releaseworkpath", ""));
                taskSetting.ReleaseWorkPath = temp0;
                var temp1 = new UserScriptableDictionary<string>();
                temp1.SetValue(GetIniSetting(tempSB, "buildParams", "exportcodepath", ""));
                taskSetting.ExportCodePath = temp1;
                var temp2 = new UserScriptableDictionary<string>();
                temp2.SetValue(GetIniSetting(tempSB, "buildParams", "apkpath", ""));
                taskSetting.ApkPath = temp2;
                var temp3 = new UserScriptableDictionary<string>();
                temp3.SetValue(GetIniSetting(tempSB, "buildParams", "aabpath", ""));
                taskSetting.AabPath = temp3;
                if (isNeedClean) BeforeBuild(taskSetting);
                StartBuild(taskSetting);
                AfterBuilt(taskSetting);
            }
        }
        catch (Exception e)
        {
            Byte[] bt = Encoding.UTF8.GetBytes(e.ToString());
            using (FileStream fs = new FileStream(Application.dataPath + "/error.log.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                fs.SetLength(0);
                fs.Write(bt, 0, bt.Length);
                fs.Close();
            }
            throw e;
        }
    }

    public static void BeforeBuild(BuildTaskSetting taskSetting)
    {
        BuildReleaseHelper.CurBuildTaskInfo = taskSetting;
        AndroidReleaseTool.ClearAndroidStudioCache();
    }

    public static void AfterBuilt(BuildTaskSetting taskSetting)
    {
        string packType = (taskSetting.PackerType.ToString().IndexOf("Debug") > -1) ? "Test" : "release";
        WritePrivateProfileString("buildOutput", "packageName", BFFramework.Utility.Text.Format("{0}_{1}_{2}{3}", GetProgramName(), taskSetting.codeVersionName, packType, taskSetting.BuildTime), _path);
        WritePrivateProfileString("buildOutput", "aabName", BFFramework.Utility.Text.Format("{0}_{1}", GetProgramName(), taskSetting.codeVersionName), _path);
        WritePrivateProfileString("buildOutput", "exportPath", EditorBuildPathHelper.GetPlayerExportCodeFolder(taskSetting), _path);
    }

    public static string GetProgramName()
    {
        string programName = (string.IsNullOrEmpty(PlayerSettings.productName)) ? "MergeLegends" : PlayerSettings.productName;
        return programName.Replace(" ", String.Empty);
    }

    private static PadType GetPadType(string input)
    {
        switch (input)
        {
            case "Google_Pad":
                return PadType.Google_Pad;
            case "Custom_Pad__Fast_follow":
                return PadType.Custom_Pad__Fast_follow;
            case "Unity_Split_Pad":
                return PadType.Unity_Split_Pad;
            default:
                return PadType.None;
        }
    }


    private static PackerType GetPackerType(string input)
    {
        switch (input)
        {
            case "Debug":
                return PackerType.Debug;
            case "Release":
                return PackerType.Release;
            default:
                return PackerType.Debug;
        }
    }

    private static OverlayInstallType GetOverlayInstallType(string input)
    {
        switch (input)
        {
            case "Force":
                return OverlayInstallType.Force;
            case "Notice":
                return OverlayInstallType.Notice;
            default:
                return OverlayInstallType.None;
        }
    }

    private static string GetIniSetting(StringBuilder temp, string section, string name, string defaultValue)
    {
        temp.Clear();
        GetPrivateProfileString(section, name, defaultValue, temp, 1024, _path);
        return temp.ToString();
    }

    public static void StartBuild(BuildTaskSetting taskSetting)
    {
        BuildReleaseHelper.RunBuild(taskSetting);
    }
}