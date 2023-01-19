using UnityEngine;

public class OrthographicCameraController : MonoBehaviour, ICameraShooter {
    private Camera m_camera;
    private string m_exportPath;
    private Vector2 m_imageResolution;
    private RenderTexture m_renderTexture;
    private Texture2D m_destinationTexture;

    public void Initialise(Camera camera, Vector2 imageResolution) {
        m_camera = camera;

        m_imageResolution = imageResolution;

        m_renderTexture = new RenderTexture(
          (int)m_imageResolution.x,
          (int)m_imageResolution.y,
          32, RenderTextureFormat.ARGB32
      );
        m_camera.targetTexture = m_renderTexture;

    }

    public void SetImageExportPath(string path) {
        m_exportPath = path;
        if (!System.IO.Directory.Exists(m_exportPath)) {
            System.IO.Directory.CreateDirectory(m_exportPath);
        }
    }

    public void FrameShotFor(GameObject go) {
        var bounds = GetGameObjectBounds(go);
        var size = bounds.size;
        float diagonal = Mathf.Sqrt(size.x * size.x + size.y * size.y + size.z * size.z);
        m_camera.orthographicSize = diagonal * 0.5f + 0.1f;
        m_camera.transform.position = bounds.center + new Vector3(0, 0, -diagonal * 0.5f);
    }

    public async void TakePhoto(string photoFileName) {

        var basicRT = RenderTexture.active;

        //Hint to the GPU to discard any previous contents of the renderTexture..
        // - It was found that artifacts were appearing in the exported images if this was not done.
        if (m_renderTexture != null) m_renderTexture.DiscardContents(true, true);

        m_renderTexture = new RenderTexture(
            (int)m_imageResolution.x,
            (int)m_imageResolution.y,
            32, RenderTextureFormat.ARGB32
        );

        //Tell the camera to render into m_renderTexture..
        m_camera.targetTexture = m_renderTexture;

        //Set the active renderTexture so that  ReadPixels will access this..
        RenderTexture.active = m_renderTexture;

        //Render the camera's view into renderTexture..
        m_camera.Render();
        m_destinationTexture = new Texture2D(
                   (int)m_imageResolution.x,
                   (int)m_imageResolution.y,
                   TextureFormat.ARGB32,
                   false
               );

        //Read the pixels from the active RentureTexture into m_destinationTexture
        m_destinationTexture.ReadPixels(
            new Rect(0, 0, (int)m_imageResolution.x, (int)m_imageResolution.y),
            0,
            0
        );

        var bytes = m_destinationTexture.EncodeToPNG();
        var filename = System.IO.Path.Combine(m_exportPath, photoFileName + ".png");
        await System.IO.File.WriteAllBytesAsync(filename, bytes);

        m_destinationTexture = null;
        RenderTexture.active = basicRT;
        photoFileName = "";
        if (m_renderTexture != null) m_renderTexture.DiscardContents(true, true);

    }

    private Bounds GetGameObjectBounds(GameObject go) {
        var bounds = new Bounds(go.transform.position, Vector3.zero);
        foreach (var renderer in go.GetComponentsInChildren<Renderer>()) {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }


}
