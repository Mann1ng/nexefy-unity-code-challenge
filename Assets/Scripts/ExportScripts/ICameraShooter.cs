using UnityEngine;
using System;


public interface ICameraShooter {

    void Initialise(Camera camera, Vector2 imageResolution);
    void FrameShotFor(GameObject go);
    void SetImageExportPath(string path);
    void TakePhoto(string photoName);
}
