using GameEngine.Shaders;
using GameEngine.Terrains;
using GameEngine.Toolbox;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace GameEngine.RenderEngine;

public class TerrainRenderer
{
    private readonly DisplayManager _displayManager;
    private readonly TerrainShader _shader;
    private readonly GL _gl;

    public TerrainRenderer(DisplayManager displayManager, TerrainShader shader, Matrix4X4<float> projectionMatrix)
    {
        _displayManager = displayManager;
        _shader = shader;
        _gl = _displayManager.GL;

        shader.Activate();
        shader.LoadProjectionMatrix(projectionMatrix);
        shader.ConnectTextures();
        shader.Deactivate();
    }

    public unsafe void Render(List<Terrain> terrains)
    {
        foreach (var terrain in terrains)
        {
            PrepareTerrain(terrain);
            LoadModelMatrix(terrain);
            _gl.DrawElements(PrimitiveType.Triangles, (uint)terrain.VAO.NumberOfIndices, DrawElementsType.UnsignedInt, null);
            UnbindTerrain(terrain);
        }
    }

    private void PrepareTerrain(Terrain terrain)
    {
        terrain.VAO.Activate();

        BindTextures(terrain);
        _shader.LoadShineVariables(1.0f, 0.0f);
    }

    private void BindTextures(Terrain terrain)
    {
        var texturePack = terrain.TexturePack;

        _gl.ActiveTexture(TextureUnit.Texture0);
        texturePack.BackgroundTexture.Activate();

        _gl.ActiveTexture(TextureUnit.Texture1);
        texturePack.RTexture.Activate();

        _gl.ActiveTexture(TextureUnit.Texture2);
        texturePack.GTexture.Activate();

        _gl.ActiveTexture(TextureUnit.Texture3);
        texturePack.BTexture.Activate();

        _gl.ActiveTexture(TextureUnit.Texture4);
        terrain.BlendMap.Activate();
    }

    private void UnbindTerrain(Terrain terrain)
    {
        terrain.VAO.Deactivate();
    }

    private void LoadModelMatrix(Terrain terrain)
    {
        Matrix4X4<float> transformationMatrix = Maths.CreateTransformationMatrix(new Vector3D<float>(terrain.X, 0.0f, terrain.Z), 0.0f, 0.0f, 0.0f, 1.0f);
        _shader.LoadTransformationMatrix(transformationMatrix);
    }
}