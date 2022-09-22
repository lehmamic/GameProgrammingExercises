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
    
    public Skeleton? Skeleton { get; set; }

    public Animation? Animation { get; set; }

    public override unsafe void Draw(Shader shader)
    {
        if (Mesh is not null)
        {
            // Set the world transform
            shader.SetUniform("uWorldTransform", Owner.WorldTransform);
            
            // Set the matrix palette
            shader.SetUniform("uMatrixPalette", _matrixPalette.Entry, MatrixPalette.MaxSkeletonBones);

            // Set specular power
            try
            {
                shader.SetUniform("uSpecPower", Mesh.SpecularPower);
            }
            catch (ShaderException)
            {
                // we catch it because the gbuffer shader has no lighting information
            }

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
        if (Animation is not null && Skeleton is not null)
        {
            _animTime += deltaTime * _animPlayRate;
            // Wrap around anim time if past duration
            while (_animTime > Animation.Duration)
            {
                _animTime -= Animation.Duration;
            }

            // Recompute matrix palette
            ComputeMatrixPalette();
        }
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
        if (Animation is not null && Skeleton is not null)
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
}