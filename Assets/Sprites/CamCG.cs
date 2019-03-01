using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CamCG : MonoBehaviour {
    public Material effectMat;

    private void OnRenderImage (RenderTexture source, RenderTexture destination) {
        Graphics.Blit (source, destination, effectMat);
    }
}