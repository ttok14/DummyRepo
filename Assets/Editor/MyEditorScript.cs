using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Build.Reporting;
using System.IO;

class MyEditorScript
{
    static string[] SCENES = FindEnabledEditorScenes();

    static string APP_NAME = "MyTestProject";
    static string TARGET_DIR = @"C:\Users\Jayce\Desktop";

    [MenuItem("Custom/CI/Build Mac OS X")]
    static void PerformMacOSXBuild()
    {
        string target_dir = APP_NAME + ".app";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneOSXIntel, BuildOptions.None);
    }

    [MenuItem("Custom/CI/Android")]
    static void PerformAndroidBuild()
    {
        string target_dir = APP_NAME + ".apk";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.Android, BuildOptions.None);


        /////////-------------------------//

        var a = System.Environment.GetCommandLineArgs();

        if (a != null && a.Length > 1)
        {
            var sr = File.CreateText("File" + DateTime.Now.Second);

            try
            {
                string prefix = "-CIBUILD:";
                string all = a.Where(t_t => t_t.Contains(prefix)).Single();
                string extracted = all.Replace(prefix, "");

                Dictionary<string, string> dataBuffer = new Dictionary<string, string>();

                string[] vars = extracted.Split(';');

                foreach (var t in vars)
                {
                    string[] pair = t.Split('=');

                    if (pair.Length != 2)
                    {
                        //    Console.WriteLine("separaator Must be =");
                        continue;
                    }

                    sr.WriteLine($"VariableName : {pair[0]} , VariableValue : {pair[1]}");

                    //     Console.WriteLine("Variable : " + pair[0] + " Is " + pair[1]);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Dispose();
                    sr = null;
                }
            }

            for (int i = 1; i < a.Length; i++)
            {
                Console.WriteLine(a[i]);
            }
        }
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
        BuildReport report = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}