using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostEffectsController : MonoBehaviour
{
    public Shader postShader;
    Material postEffectMaterial;

    RenderTexture postRenderTexture;

    public void Awake()
    {
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postEffectMaterial == null)
        {
            postEffectMaterial = new Material(postShader);
        }
        if (postRenderTexture == null)
        {
            postRenderTexture = new RenderTexture(source.width, source.height, 0, source.format);
        }

        Graphics.Blit(source, postRenderTexture, postEffectMaterial, 0);
        Shader.SetGlobalTexture
                 ("_GlobalRenderTexture", postRenderTexture);
        Graphics.Blit(source, destination);
    }
}
