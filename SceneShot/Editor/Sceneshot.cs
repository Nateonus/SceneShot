/*
 * 
 * Created by Nathaniel Ford (Nateonus Apps)
 * https://twitter.com/nateonus
 * 13/03/2020
 * Licenced CC0.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Sceneshot : EditorWindow
{

    public static int ssWidth = 1920;
    public static int ssHeight = 1080;
    public static bool openOnFinish = true;

    //Quality Settings.
    public static AntiAliasingLevel antialiasingLevel = AntiAliasingLevel.High;
    public static AntiAliasingLevel anisotropicLevel = AntiAliasingLevel.Ultra;

    public static string dataPath = "screenshots//";    

    [MenuItem("SceneShot//Take Quick Screenshot")]
    public static void TakeQuickScreenshot()
    {
        LoadData();
        Camera[] sceneCameras = SceneView.GetAllSceneCameras();
        if (sceneCameras.Length > 1)
        {
            Debug.Log("Multiple scene view cameras exist. Multiple screenshots will be taken.");
        }
        else if (sceneCameras.Length == 0)
        {
            Debug.LogError("No scene view cameras found.");
            return;
        }

        RenderTexture cameraRender = new RenderTexture(ssWidth,ssHeight,24);
        cameraRender.antiAliasing = (int)antialiasingLevel;
        cameraRender.anisoLevel = (int)anisotropicLevel;
        Texture2D screenShot = new Texture2D(ssWidth, ssHeight, TextureFormat.RGB24, false);
        foreach (Camera c in sceneCameras)
        {
            c.targetTexture = cameraRender;
            c.Render();
            cameraRender.ResolveAntiAliasedSurface();
            c.targetTexture = null;
            RenderTexture.active = cameraRender;
            screenShot.ReadPixels(new Rect(0, 0, ssWidth, ssHeight), 0, 0);
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = dataPath + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")+".png";
            if (!System.IO.Directory.Exists(dataPath))
            {
                System.IO.Directory.CreateDirectory(dataPath);
            }
            System.IO.File.WriteAllBytes(filename, bytes);
            if (openOnFinish)
            {
                Application.OpenURL(System.IO.Directory.GetParent(Application.dataPath)+"//"+filename);
            }
            Debug.Log(string.Format("Took screenshot to: {0}", System.IO.Directory.GetParent(Application.dataPath) + "//" + filename));
        }
    }

    static void LoadData()
    {
        ssWidth = EditorPrefs.GetInt("screenshotwidth", ssWidth);
        ssHeight = EditorPrefs.GetInt("screenshotheight", ssHeight);
        openOnFinish = EditorPrefs.GetBool("screenshotopenonfinish", openOnFinish);
        antialiasingLevel = (AntiAliasingLevel)EditorPrefs.GetInt("screenshotantialiasinglevel", (int)antialiasingLevel);
        anisotropicLevel = (AntiAliasingLevel)EditorPrefs.GetInt("screenshotanisolevel", (int)anisotropicLevel);
    }

    private void Awake()
    {
        LoadData();
    }

    [MenuItem("SceneShot//Configure Screenshot")]
    public static void OpenConfigureScreenshotMenu()
    {
        GetWindow(typeof(Sceneshot)).Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Config Settings", EditorStyles.boldLabel);
        ssWidth = EditorGUILayout.IntField("Screenshot Width", ssWidth);
        ssHeight = EditorGUILayout.IntField("Screenshot Height", ssHeight);

        if (GUILayout.Button("Set to current scene view resolution."))
        {
            if (SceneView.GetAllSceneCameras().Length < 0)
            {
                Debug.Log("No scene view found.");
            }
            ssWidth = (int)SceneView.GetAllSceneCameras()[0].activeTexture.width;
            ssHeight = (int)SceneView.GetAllSceneCameras()[0].activeTexture.height;
        }

        if (GUILayout.Button("Set to 2x current scene view resolution."))
        {
            if (SceneView.GetAllSceneCameras().Length < 0)
            {
                Debug.Log("No scene view found.");
            }
            ssWidth = (int)SceneView.GetAllSceneCameras()[0].activeTexture.width * 2;
            ssHeight = (int)SceneView.GetAllSceneCameras()[0].activeTexture.height * 2;
        }

        if (GUILayout.Button("Set to 4x current scene view resolution."))
        {
            if (SceneView.GetAllSceneCameras().Length < 0)
            {
                Debug.Log("No scene view found.");
            }
            ssWidth = (int)SceneView.GetAllSceneCameras()[0].activeTexture.width * 4;
            ssHeight = (int)SceneView.GetAllSceneCameras()[0].activeTexture.height * 4;
        }

        if (GUILayout.Button("Set to 8x current scene view resolution."))
        {
            if (SceneView.GetAllSceneCameras().Length < 0)
            {
                Debug.Log("No scene view found.");
            }
            ssWidth = (int)SceneView.GetAllSceneCameras()[0].activeTexture.width * 8;
            ssHeight = (int)SceneView.GetAllSceneCameras()[0].activeTexture.height * 8;
        }

        openOnFinish = EditorGUILayout.Toggle("Open on capture?", openOnFinish);


        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Quality Settings", EditorStyles.boldLabel);
        antialiasingLevel = (AntiAliasingLevel)EditorGUILayout.EnumPopup("Anti Aliasing Level", antialiasingLevel as System.Enum);
        anisotropicLevel = (AntiAliasingLevel)EditorGUILayout.EnumPopup("Anisotropic Filtering Level", anisotropicLevel as System.Enum);

        EditorGUILayout.Separator();
        if (GUILayout.Button("Take Screenshot"))
        {
            SaveInfo();
            TakeQuickScreenshot();
        }
    }

    private void SaveInfo()
    {
        EditorPrefs.SetInt("screenshotwidth", ssWidth);
        EditorPrefs.SetInt("screenshotheight", ssHeight);
        EditorPrefs.SetBool("screenshotopenonfinish", openOnFinish);
        EditorPrefs.SetInt("screenshotantialiasinglevel", (int)antialiasingLevel);
        EditorPrefs.SetInt("screenshotanisolevel", (int)anisotropicLevel);
    }

    private void OnDestroy()
    {
        SaveInfo();
    }

}


public enum AntiAliasingLevel
{
    None = 1,
    Low = 2,
    Medium = 4,
    High = 8,
    Ultra = 16
}