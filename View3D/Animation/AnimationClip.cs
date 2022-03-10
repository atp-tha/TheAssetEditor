﻿using CommonControls.FileTypes.Animation;
using CommonControls.FileTypes.RigidModel.Transforms;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static CommonControls.FileTypes.Animation.AnimationFile;


namespace View3D.Animation
{
    public class AnimationClip
    {
        public class KeyFrame
        {
            public List<Vector3> Position { get; set; } = new List<Vector3>();
            public List<Quaternion> Rotation { get; set; } = new List<Quaternion>();
            public List<Vector3> Scale { get; set; } = new List<Vector3>();

            public override string ToString()
            {
                return $"PosCount = {Position.Count}, RotCount = {Rotation.Count}, ScaleCount = {Scale.Count}";
            }

            public KeyFrame Clone()
            {
                return new KeyFrame()
                {
                    Position = new List<Vector3>(Position),
                    Rotation = new List<Quaternion>(Rotation),
                    Scale = new List<Vector3>(Scale)
                };
            }
        }


        public List<KeyFrame> DynamicFrames = new List<KeyFrame>();
        public float PlayTimeInSec { get; set; } = -1;
        public int AnimationBoneCount
        {
            get 
            {
                if (DynamicFrames.Count == 0)
                    return 0;
                return DynamicFrames[0].Position.Count;
            }
        }


        public AnimationClip() { }

        public AnimationClip(AnimationFile file, GameSkeleton skeleton)
        {
            foreach (var animationPart in file.AnimationParts)
            {
                var frames = CreateKeyFramesFromAnimationPart(animationPart, skeleton);
                DynamicFrames.AddRange(frames);
            }

            PlayTimeInSec = file.Header.AnimationTotalPlayTimeInSec;
        }


        List<KeyFrame> CreateKeyFramesFromAnimationPart(AnimationPart animationPart, GameSkeleton skeleton)
        {
            List<KeyFrame> newDynamicFrames = new List<KeyFrame>();

            var boneCount = animationPart.RotationMappings.Count;
            var frameCount = animationPart.DynamicFrames.Count;

            for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
            {
                var newKeyframe = new KeyFrame();

                for (int boneIndex = 0; boneIndex < boneCount; boneIndex++)
                {
                    var translationLookup = animationPart.TranslationMappings[boneIndex];
                    if (translationLookup.IsDynamic)
                        newKeyframe.Position.Add(animationPart.DynamicFrames[frameIndex].Transforms[translationLookup.Id].ToVector3());
                    else if (translationLookup.IsStatic)
                        newKeyframe.Position.Add(animationPart.StaticFrame.Transforms[translationLookup.Id].ToVector3());
                    else
                        newKeyframe.Position.Add(skeleton.Translation[boneIndex]);

                    var rotationLookup = animationPart.RotationMappings[boneIndex];
                    if (rotationLookup.IsDynamic)
                        newKeyframe.Rotation.Add(animationPart.DynamicFrames[frameIndex].Quaternion[rotationLookup.Id].ToQuaternion());
                    else if (rotationLookup.IsStatic)
                        newKeyframe.Rotation.Add(animationPart.StaticFrame.Quaternion[rotationLookup.Id].ToQuaternion());
                    else
                        newKeyframe.Rotation.Add(skeleton.Rotation[boneIndex]);

                    newKeyframe.Scale.Add(Vector3.One);
                }

                newDynamicFrames.Add(newKeyframe);
            }

            return newDynamicFrames;
        }

        public AnimationFile ConvertToFileFormat(GameSkeleton skeleton)
        {
            throw new NotImplementedException();
            /*
            AnimationFile output = new AnimationFile();

            var fRate = (DynamicFrames.Count() - 1) / PlayTimeInSec;
            output.Header.FrameRate = (float)Math.Floor(fRate);
            if (output.Header.FrameRate <= 0)
                output.Header.FrameRate = 20;

            output.Header.Version = 7;
            output.Header.AnimationTotalPlayTimeInSec = PlayTimeInSec;
            output.Header.SkeletonName = skeleton.SkeletonName;

            output.Bones = new BoneInfo[skeleton.BoneCount];
            for (int i = 0; i < skeleton.BoneCount; i++)
            {
                output.Bones[i] = new BoneInfo()
                {
                    Id = i,
                    Name = skeleton.BoneNames[i],
                    ParentId = skeleton.GetParentBoneIndex(i)
                };
            }

            // Mappings
            output.RotationMappings = RotationMappings.ToList();
            output.TranslationMappings = TranslationMappings.ToList();

            // Static
            if (StaticFrame != null)
                output.StaticFrame = CreateFrameFromKeyFrame(StaticFrame);

            // Dynamic
            foreach (var frame in DynamicFrames)
                output.DynamicFrames.Add(CreateFrameFromKeyFrame(frame));

            return output;
            */
        }

        public AnimationClip Clone()
        {
            AnimationClip copy = new AnimationClip();
            copy.PlayTimeInSec = PlayTimeInSec;

            foreach (var item in DynamicFrames)
                copy.DynamicFrames.Add(item.Clone());

            return copy;
        }

        public static AnimationClip CreateSkeletonAnimation(GameSkeleton skeleton)
        {
            var clip = new AnimationClip()
            {
                PlayTimeInSec = 0.1f,
            };

            var frame = new KeyFrame();
            for (int i = 0; i < skeleton.BoneCount; i++)
            {
                frame.Position.Add(skeleton.Translation[i]);
                frame.Rotation.Add(skeleton.Rotation[i]);
                frame.Scale.Add(Vector3.One);
            }

            // Skeletons have two identical frames, dont know why
            clip.DynamicFrames.Add(frame.Clone());
            clip.DynamicFrames.Add(frame.Clone());
            return clip;
        }
    }
}
