using System.Text.Json;
using System.Text.Json.Serialization;
using Silk.NET.Maths;

namespace GameProgrammingExercises;

public class Animation
{
    private Animation(BoneTransform[][] tracks, uint numBones, uint numFrames, float duration, float frameDuration)
    {
        Tracks = tracks;
        NumBones = numBones;
        NumFrames = numFrames;
        Duration = duration;
        FrameDuration = frameDuration;
    }

    public BoneTransform[][] Tracks { get; }

    public uint NumBones { get; }

    public uint NumFrames { get; }

    public float Duration { get; }

    public float FrameDuration { get; }

    public static Animation Load(string fileName)
    {
        var jsonString = File.ReadAllText(fileName);
        var raw = JsonSerializer.Deserialize<RawAnimation>(jsonString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        if (raw?.Version != 1)
        {
            throw new AnimationException($"Animation {fileName} is not version 1.");
        }

        var numFrames = raw.Sequence.Frames;
        var duration = raw.Sequence.Length;
        var numBones = raw.Sequence.BoneCount;
        var frameDuration = duration / (numFrames - 1);

        var tracks = Enumerable.Repeat(Array.Empty<BoneTransform>(), (int) numBones)
            .ToArray();
        for (var i = 0; i < raw.Sequence.Tracks.Length; i++)
        {
            var boneIndex = raw.Sequence.Tracks[i].Bone;

            if (raw.Sequence.Tracks[i].Transforms.Length < numFrames)
            {
                throw new AnimationException($"Animation {fileName}: Track element {i} has fewer frames than expected.");
            }

            tracks[boneIndex] = new BoneTransform[raw.Sequence.Tracks[i].Transforms.Length];
            for (var j = 0; j < raw.Sequence.Tracks[i].Transforms.Length; j++)
            {
                var rot = raw.Sequence.Tracks[i].Transforms[j].Rot;
                var trans = raw.Sequence.Tracks[i].Transforms[j].Trans;

                BoneTransform temp = new();
                temp.Rotation.X = rot[0];
                temp.Rotation.Y = rot[1];
                temp.Rotation.Z = rot[2];
                temp.Rotation.W = rot[3];
                
                temp.Translation.X = trans[0];
                temp.Translation.Y = trans[1];
                temp.Translation.Z = trans[2];

                tracks[boneIndex][j] = temp;
            }
        }

        return new Animation(tracks, numBones, numFrames, duration, frameDuration);
    }

    /// <summary>
    /// Fills the provided vector with the global (current) pose matrices for each
    /// bone at the specified time in the animation. It is expected that the time
    /// is >= 0.0f and <= mDuration
    /// </summary>
    /// <param name="skeleton"></param>
    /// <param name="time"></param>
    public Matrix4X4<float>[] GetGlobalPoseAtTime(Skeleton skeleton, float time)
    {
        var outPoses = new Matrix4X4<float>[NumBones];

        // Figure out the current frame index and next frame
        // (This assumes inTime is bounded by [0, AnimDuration]
        var frame = (int)(time / FrameDuration);
        var nextFrame = frame + 1;
        // Calculate fractional value between frame and next frame
        float pct = time / FrameDuration - frame;

        // Setup the pose for the root
        if (Tracks[0].Length > 0)
        {
            // Interpolate between the current frame's pose and the next frame
            BoneTransform interp = BoneTransform.Interpolate(Tracks[0][frame], Tracks[0][nextFrame], pct);
            outPoses[0] = interp.ToMatrix();
        }
        else
        {
            outPoses[0] = Matrix4X4<float>.Identity;
        }

        var bones = skeleton.Bones;
        // Now setup the poses for the rest
        for (var bone = 1; bone < NumBones; bone++)
        {
            var localMat = Matrix4X4<float>.Identity; // (Defaults to identity)
            if (Tracks[bone].Length > 0)
            {
                BoneTransform interp = BoneTransform.Interpolate(Tracks[bone][frame], Tracks[bone][nextFrame], pct);
                localMat = interp.ToMatrix();
            }

            outPoses[bone] = localMat * outPoses[bones[bone].Parent];
        }

        return outPoses;
    }

    private class RawAnimation
    {
        public int Version { get; set; }
        
        public RawSequence Sequence { get; set; }
    }

    private class RawSequence
    {
        public uint Frames { get; set; }

        public float Length { get; set; }

        [JsonPropertyName("bonecount")]
        public uint BoneCount { get; set; }

        public RawTrack[] Tracks  { get; set; }
    }

    private class RawTrack
    {
        public int Bone { get; set; }
        
        public RawPose[] Transforms { get; set; }
    }
    
    private class RawPose
    {
        public float[] Rot { get; set; }

        public float[] Trans { get; set; }
    }
}