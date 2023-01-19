using UnityEngine;
using System.IO;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(ISpawnEnumerator))]

public class ExportController : MonoBehaviour {
    [SerializeField]
    private int bulletTimeFrameCount = 16;

    [SerializeField]
    private Vector2 imageResolution;

    [SerializeField]
    private string frameNameTemplate = "frame{index}";

    [SerializeField]
    private string exportFolderName = "Output";

    [SerializeField] private bool doWaitForEndOfFrames = true;
    [SerializeField] private bool useExistingCamera = false;
    [SerializeField] private bool useExistingLighting = false;
    private string m_exportRootPath = "";

    private Camera m_camera = null;
    private ISpawnEnumerator m_spawner;

    private ICameraShooter m_cameraShooter;

    void Start() {
        Initialise();
        DoNextShoot();
    }

    void Initialise() {

        if (imageResolution == default(Vector2)) {
            imageResolution = Vector2.one * 512f;
        }
        if (string.IsNullOrEmpty(exportFolderName)) {
            exportFolderName = "Output";
        }
        m_exportRootPath = System.IO.Path.Combine(Path.GetDirectoryName(Application.dataPath), exportFolderName);

        if (!useExistingLighting) { SetupLighting(); }

        m_camera = Camera.main;
        if (m_camera == null || !useExistingCamera) { m_camera = CreateCamera(); }

        m_spawner = GetComponent<ISpawnEnumerator>();
        m_cameraShooter = GetComponent<ICameraShooter>();
        m_cameraShooter.Initialise(m_camera, imageResolution);

        //If we want to see each frame of the shoot in game view, then set the main Camera to shadow the render camera..
        if (doWaitForEndOfFrames && Camera.main != m_camera) {
            var shadowCam = Camera.main.gameObject.AddComponent<ShadowCamera>();
            shadowCam.SetParentCamera(m_camera.gameObject);
        }

    }

    void DoNextShoot() {
        //Spawn the next model and shoot it; OR if no more models show export and stop playing...
        m_spawner.SpawnNext((go) => {
            DoShootBulletTime(go, bulletTimeFrameCount, DoNextShoot);
        }, DoExportFinished);
    }

    private void DoShootBulletTime(GameObject go, int frameCount = 16, Action onDone = null) {
        StartCoroutine(DoShootBulletTime_Coroutine(go, frameCount, onDone));
    }

    private IEnumerator DoShootBulletTime_Coroutine(GameObject go, int frameCount = 16, Action onDone = null) {
        var gazePos = -m_camera.transform.position; gazePos.y = 0;
        go.transform.LookAt(gazePos);
        m_cameraShooter.FrameShotFor(go);
        m_cameraShooter.SetImageExportPath(System.IO.Path.Combine(m_exportRootPath, go.name));

        float degPerFrame = 360f / frameCount;
        for (int i = 0; i < frameCount; i++) {
            m_cameraShooter.TakePhoto(frameNameTemplate.Replace("{index}", i.ToString("D4")));
            go.transform.Rotate(Vector3.up, degPerFrame);
            if (doWaitForEndOfFrames) { yield return null; }
        }

        if (onDone != null) { onDone(); }

    }

    void DoExportFinished() {
        m_spawner.Reset();
        Application.OpenURL($"file://" + m_exportRootPath);
        Debug.Log("Export finished!");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private Camera CreateCamera() {
        var camera = new GameObject("_PhotoCaptureCamera").AddComponent<Camera>();
        camera.transform.position = new Vector3(0, 0, -11.11f);
        camera.transform.rotation = Quaternion.identity;
        camera.orthographic = true;
        camera.orthographicSize = 5;
        camera.nearClipPlane = 0.3f;
        camera.farClipPlane = 1000;
        camera.depth = -1;
        camera.clearFlags = CameraClearFlags.Depth;
        camera.allowMSAA = true;
        camera.allowHDR = false;
        camera.tag = "MainCamera"; //incase there is no other in the scene..
        return camera;
    }

    private Light SetupLighting() {
        //disabled other lights in scene 
        var lights = FindObjectsOfType<Light>();
        foreach (var l in lights) { l.enabled = false; }

        var light = new GameObject("_PhotoCaptureLight").AddComponent<Light>();
        light.type = LightType.Directional;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 1;
        light.shadowBias = 0.05f;
        light.shadowNormalBias = 0.4f;
        light.shadowNearPlane = 0.2f;
        light.renderMode = LightRenderMode.ForcePixel;
        light.cullingMask = 1 << 0;
        return light;
    }

}
