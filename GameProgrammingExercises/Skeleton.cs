using System.Text.Json;
using System.Text.Json.Serialization;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Skeleton
{
    private Matrix4X4<float>[] _globalInvBindPoses;

    public Skeleton(Bone[] bones)
    {
        Bones = bones;
    }

    public Bone[] Bones { get; }

    public int NumBones => Bones.Length;

    public Matrix4X4<float>[] GlobalInvBindPoses => _globalInvBindPoses;

    public static Skeleton Load(string fileName)
    {
        var jsonString = File.ReadAllText(fileName);
        var raw = JsonSerializer.Deserialize<RawSkeleton>(jsonString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (raw?.Version != 1)
        {
            throw new SkeletonException($"Skeleton {fileName} is not version 1.");
        }

        var count = raw.BoneCount;

        if (count > MatrixPalette.MaxSkeletonBones)
        {
            throw new SkeletonException($"Skeleton {fileName} exceeds maximum bone count.");
        }

        if (raw.Bones.Length != count)
        {
            throw new SkeletonException($"Skeleton {fileName} has a mismatch between the bone count and number of bones");
        }

        var bones = new Bone[count];
        for (int i = 0; i < count; i++)
        {
            var temp = new Bone();
            temp.Name = raw.Bones[i].Name;
            temp.Parent = raw.Bones[i].Parent;
            
            temp.LocalBindPose.Rotation.X = raw.Bones[i].BindPose.Rot[0];
            temp.LocalBindPose.Rotation.Y = raw.Bones[i].BindPose.Rot[1];
            temp.LocalBindPose.Rotation.Z = raw.Bones[i].BindPose.Rot[2];
            temp.LocalBindPose.Rotation.W = raw.Bones[i].BindPose.Rot[3];

            temp.LocalBindPose.Translation.X = raw.Bones[i].BindPose.Trans[0];
            temp.LocalBindPose.Translation.Y = raw.Bones[i].BindPose.Trans[1];
            temp.LocalBindPose.Translation.Z = raw.Bones[i].BindPose.Trans[2];
            bones[i] = temp;
        }

        var skeleton = new Skeleton(bones.ToArray());
        skeleton.ComputeGlobalInvBindPose();

        return skeleton;
    }

    protected void ComputeGlobalInvBindPose()
    {
        // Resize to number of bones, which automatically fills identity
        _globalInvBindPoses = Enumerable
            .Repeat(Matrix4X4<float>.Identity, Bones.Length)
            .ToArray();

        // Step 1: Compute global bind pose for each bone

        // The global bind pose for root is just the local bind pose
        _globalInvBindPoses[0] = Bones[0].LocalBindPose.ToMatrix();

        // Each remaining bone's global bind pose is its local pose
        // multiplied by the parent's global bind pose
        for (var i = 1; i < Bones.Length; i++)
        {
            Matrix4X4<float> localMat = Bones[i].LocalBindPose.ToMatrix();
            _globalInvBindPoses[i] = localMat * _globalInvBindPoses[Bones[i].Parent];
        }

        // Step 2: Invert
        for (var i = 0; i < _globalInvBindPoses.Length; i++)
        {
            if (Matrix4X4.Invert(_globalInvBindPoses[i], out var invertedBindPose))
            {
                _globalInvBindPoses[i] = invertedBindPose;
            }
        }
    }

    private class RawSkeleton
    {
        public int Version { get; set; }

        [JsonPropertyName("bonecount")]
        public uint BoneCount { get; set; }

        public RawBone[] Bones { get; set; }
    }

    private class RawBone
    {
        public string Name { get; set; }
        
        public int Parent { get; set; }
        
        [JsonPropertyName("bindpose")]
        public RawBindPose BindPose { get; set; }
    }

    private class RawBindPose
    {
        public float[] Rot { get; set; }

        public float[] Trans { get; set; }
    }
}