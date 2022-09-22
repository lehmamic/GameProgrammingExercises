using Silk.NET.OpenGL;

namespace GameProgrammingExercises;

public class SkeletalMeshComponent : MeshComponent
{
    private MatrixPalette _matrixPalette;
    private float _animPlayRate;
    private float _animTime;
    
    public SkeletalMeshComponent(Actor owner)
        : base(owner, true)
    {
    }
    
    public Skeleton Skeleton { get; set; }

    public Animation Animation { get; set; }

    public override unsafe void Draw(Shader shader)
    {
        if (Mesh is not null)
        {
            // Set the world transform
            shader.SetUniform("uWorldTransform", Owner.WorldTransform);
            
            // Set the matrix palette
            shader.SetUniform("uMatrixPalette", _matrixPalette.Entry, MatrixPalette.MaxSkeletonBones);

            // Set specular power
            shader.SetUniform("uSpecPower", Mesh.SpecularPower);

            // Set the active texture
            var texture = Mesh.GetTexture(TextureIndex);
            if (texture is not null)
            {
                texture.SetActive();
            }

            // Set mesh's vertex array as active
            var vao = Mesh.VertexArray;
            vao.SetActive();

            // Draw
            Owner.Game.Renderer.GL.DrawElements(PrimitiveType.Triangles, (uint)vao.NumberOfIndices, DrawElementsType.UnsignedInt, null);
        }
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
    
    public float PlayAnimation(Animation anim, float playRate = 1.0f)
    {
        Animation = anim;
        _animTime = 0.0f;
        _animPlayRate = playRate;

        if (Animation is null)
        {
            return 0.0f;
        }

        ComputeMatrixPalette();

        return Animation.Duration;
    }

    private void ComputeMatrixPalette()
    {
        var globalInvBindPoses = Skeleton.GlobalInvBindPoses;
        var currentPoses = Animation.GetGlobalPoseAtTime(Skeleton, _animTime);

        _matrixPalette = new MatrixPalette();
        // Setup the palette for each bone
        for (var i = 0; i < Skeleton.NumBones; i++)
        {
            // Global inverse bind pose matrix times current pose matrix
            _matrixPalette.Entry[i] = globalInvBindPoses[i] * currentPoses[i];
        }
    }
}