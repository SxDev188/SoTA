using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RadialColorRenderFeature : ScriptableRendererFeature
{
    class RadialColorRenderPass : ScriptableRenderPass
    {
        private Material material;
        private RenderTargetHandle tempTexture;
        private Vector4 playerPosition = new Vector4(0, 0, 0, 0); // Default

        public RadialColorRenderPass(Material mat)
        {
            this.material = mat;
            tempTexture.Init("_TemporaryColorTexture");
        }

        public void SetPlayerPosition(Vector3 screenPos)
        {
            playerPosition = new Vector4(screenPos.x, screenPos.y, 0, 0);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempTexture.id, cameraTextureDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Radial Color Effect");

            // Get camera target
            RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTargetHandle;

            // Pass player position to shader
            material.SetVector("_PlayerPosition", playerPosition);
            Resolution resolution = Screen.currentResolution;
            Vector2 screenResolution = new Vector2(resolution.width, resolution.height);
            material.SetVector("_ScreenResolution", screenResolution);

            // Apply effect
            cmd.Blit(source, tempTexture.Identifier(), material);
            cmd.Blit(tempTexture.Identifier(), source, material, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    private RadialColorRenderPass pass;
    private Material material;
    private static readonly string ShaderName = "Unlit/RadialColorMaskURP";

    public override void Create()
    {
        Shader shader = Shader.Find(ShaderName);
        if (shader == null)
        {
            Debug.LogError("RadialColorRenderFeature: Shader is missing!");
            return;
        }

        material = CoreUtils.CreateEngineMaterial(shader);
        pass = new RadialColorRenderPass(material)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (pass != null)
        {
            renderer.EnqueuePass(pass);
        }
    }

    public void SetPlayerPosition(Vector3 playerScreenPos)
    {
        if (pass != null)
        {
            pass.SetPlayerPosition(playerScreenPos);
        }
    }
}