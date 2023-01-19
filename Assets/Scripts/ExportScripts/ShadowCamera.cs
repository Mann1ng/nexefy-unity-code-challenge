using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ShadowCamera : MonoBehaviour {
    private GameObject m_parentCamera;

    public void SetParentCamera(GameObject parentCamera) {
        m_parentCamera = parentCamera;
        this.transform.position = m_parentCamera.transform.position;
        this.transform.rotation = m_parentCamera.transform.rotation;
        this.transform.SetParent(m_parentCamera.transform);

    }
}
