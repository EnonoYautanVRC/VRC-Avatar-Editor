﻿using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRCAvatarEditor.Base;
using VRCAvatarEditor.Interfaces;
using VRCAvatar = VRCAvatarEditor.Base.VRCAvatarBase;

// Copyright (c) 2019 gatosyocora

namespace VRCAvatarEditor
{

    public class FaceEmotion : IFaceEmotion
    {
        public readonly string SENDDATAASSET_PATH = "Assets/SendData.asset";

        /// <summary>
        /// 指定したBlendShapeのアニメーションファイルを作成する
        /// </summary>
        public AnimationClip CreateBlendShapeAnimationClip(string fileName, string saveFolderPath, VRCAvatar avatar)
        {
            AnimationClip animClip = new AnimationClip();

            if (fileName == "") fileName = "face_emotion";

            foreach (IFaceEmotionSkinnedMesh skinnedMesh in avatar.SkinnedMeshList)
            {
                if (!skinnedMesh.IsOpenBlendShapes || skinnedMesh.BlendShapeCount <= 0) continue;

                string path = AnimationUtility.CalculateTransformPath(skinnedMesh.Renderer.transform, avatar.Animator.transform);

                foreach (var blendshape in skinnedMesh.Blendshapes)
                {
                    float keyValue;
                    AnimationCurve curve = new AnimationCurve();
                    if (!blendshape.IsExclusion && blendshape.IsContains)
                    {
                        keyValue = skinnedMesh.Renderer.GetBlendShapeWeight(blendshape.Id);

                        curve.keys = null;

                        curve.AddKey(0, keyValue);
                        curve.AddKey(1 / 60.0f, keyValue);

                        animClip.SetCurve(path, typeof(SkinnedMeshRenderer), "blendShape." + blendshape.Name, curve);

                    }
                }
            }

            var saveFilePath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(saveFolderPath, $"{fileName}.anim")
            );
            AssetDatabase.CreateAsset(animClip, saveFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return animClip;
        }

        /// <summary>
        /// BlendShapeの値をすべてリセットする
        /// </summary>
        public void ResetAllBlendShapeValues(VRCAvatar avatar)
        {
            foreach (var skinnedMesh in avatar.SkinnedMeshList)
            {
                if (!skinnedMesh.IsOpenBlendShapes || skinnedMesh.BlendShapeCount <= 0) continue;

                foreach (var blendshape in skinnedMesh.Blendshapes)
                    if (blendshape.IsContains)
                        SetBlendShapeMinValue(skinnedMesh.Renderer, blendshape.Id);
            }
        }

        /// <summary>
        /// BlendShapeの値を最大値にする
        /// </summary>
        public bool SetBlendShapeMaxValue(SkinnedMeshRenderer renderer, int id)
        {
            float maxValue = 0f;

            if (renderer.sharedMesh != null)
            {
                var mesh = renderer.sharedMesh;
                if (mesh == null) return false;

                for (int frameIndex = 0; frameIndex < mesh.GetBlendShapeFrameCount(id); frameIndex++)
                    maxValue = Mathf.Max(maxValue, mesh.GetBlendShapeFrameWeight(id, frameIndex));

                renderer.SetBlendShapeWeight(id, maxValue);

                return true;
            }

            return false;
        }

        /// <summary>
        /// BlendShapeの値を最小値にする
        public bool SetBlendShapeMinValue(SkinnedMeshRenderer renderer, int id)
        {
            float minValue = 0f;

            if (renderer.sharedMesh != null)
            {
                var mesh = renderer.sharedMesh;
                if (mesh == null) return false;

                for (int frameIndex = 0; frameIndex < mesh.GetBlendShapeFrameCount(id); frameIndex++)
                    minValue = Mathf.Min(minValue, mesh.GetBlendShapeFrameWeight(id, frameIndex));

                renderer.SetBlendShapeWeight(id, minValue);

                return true;
            }
            return false;
        }

        /// <summary>
        /// すべてのBlendshapeのisContainの値を変更する
        /// </summary>
        /// <param name="value"></param>
        /// <param name="blendshapes"></param>
        public bool SetContainsAll(bool value, List<SkinnedMesh.BlendShape> blendshapes)
        {
            if (blendshapes == null) return false;

            foreach (var blendshape in blendshapes)
                blendshape.IsContains = value;

            return true;
        }

        /// <summary>
        /// アニメーションファイルを選択し, BlendShapeのデータを読み込む
        /// 読み込んだデータはScriptableSingleton<SendData>に格納される
        /// </summary>
        /// <param name="sendData"></param>
        /// <param name="preWindow"></param>
        public void LoadAnimationProperties(FaceEmotionViewBase faceEmotionGUI, VRCAvatarEditorView editorGUI)
        {

            string animFilePath = EditorUtility.OpenFilePanel("Select Loading Animation File", "Assets", "anim");

            if (animFilePath == "") return;

            animFilePath = FileUtil.GetProjectRelativePath(animFilePath);

            ScriptableSingleton<SendData>.instance.filePath = animFilePath;

            AnimationLoaderView.OnLoadedAnimationProperties -= faceEmotionGUI.OnLoadedAnimationProperties;
            AnimationLoaderView.OnLoadedAnimationProperties += faceEmotionGUI.OnLoadedAnimationProperties;
            editorGUI.OpenSubWindow();
        }

        /// <summary>
        /// 選択したblendshapeのプロパティを反映する
        /// </summary>
        /// <param name="animProperties"></param>
        /// <param name="skinnedMeshes"></param>
        public void ApplyAnimationProperties(List<AnimParam> animProperties, VRCAvatar avatar)
        {
            for (int skinnedMeshIndex = 0; skinnedMeshIndex < avatar.SkinnedMeshList.Count; skinnedMeshIndex++)
            {
                IFaceEmotionSkinnedMesh skinnedMesh = avatar.SkinnedMeshList[skinnedMeshIndex];
                if (skinnedMesh.BlendShapeCount <= 0) continue;

                var mesh = skinnedMesh.Mesh;
                var renderer = skinnedMesh.Renderer;
                var blendshapes = skinnedMesh.Blendshapes;

                if (renderer == null) continue;

                // 一旦すべてリセットする
                for (int i = 0; i < avatar.DefaultFaceEmotion.Count; i++)
                {
                    var defaultAnimProperty = avatar.DefaultFaceEmotion[i];
                    var index = mesh.GetBlendShapeIndex(defaultAnimProperty.BlendShapeName);
                    if (index < 0) continue;
                    renderer.SetBlendShapeWeight(index, defaultAnimProperty.Value);
                    blendshapes[index].IsContains = false;
                }

                // アニメーションファイルに含まれるものだけ値を変更して
                // 適用させるチェックマークにチェックをいれる
                foreach (var animProperty in animProperties)
                {
                    var index = mesh.GetBlendShapeIndex(animProperty.BlendShapeName);
                    if (index >= 0)
                    {
                        renderer.SetBlendShapeWeight(index, animProperty.Value);
                        blendshapes[index].IsContains = true;
                    }
                }
            }
        }

        public void ApplyAnimationProperties(AnimationClip clip, VRCAvatar avatar)
        {
            var paramList = GetAnimationParamaters(clip);
            ApplyAnimationProperties(paramList, avatar);
        }

        public void SetToDefaultFaceEmotion(VRCAvatar editAvatar, VRCAvatar originalAvatar)
        {
            var defaultFaceEmotion = GetAvatarFaceParamaters(editAvatar.SkinnedMeshList);
            editAvatar.DefaultFaceEmotion = defaultFaceEmotion;
            ApplyAnimationProperties(defaultFaceEmotion, originalAvatar);
        }

        public void ResetToDefaultFaceEmotion(VRCAvatar avatar)
        {
            if (avatar == null) return;

            ApplyAnimationProperties(avatar.DefaultFaceEmotion, avatar);
        }

        public List<AnimParam> GetAnimationParamaters(AnimationClip clip)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);
            var animParamList = new List<AnimParam>();

            foreach (var binding in bindings)
            {
                if ((binding.propertyName).Split('.')[0] != "blendShape") continue;
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                var animParam = new AnimParam(binding.path, binding.propertyName, curve[0].value);
                animParamList.Add(animParam);
            }

            return animParamList;
        }

        public List<AnimParam> GetAvatarFaceParamaters(List<SkinnedMesh> skinnedMeshList)
        {
            var animParamList = new List<AnimParam>();

            for (int skinnedMeshIndex = 0; skinnedMeshIndex < skinnedMeshList.Count; skinnedMeshIndex++)
            {
                IFaceEmotionSkinnedMesh skinnedMesh = skinnedMeshList[skinnedMeshIndex];
                if (skinnedMesh.BlendShapeCount <= 0) continue;

                var mesh = skinnedMesh.Mesh;
                var renderer = skinnedMesh.Renderer;

                for (int blendShapeIndex = 0; blendShapeIndex < mesh.blendShapeCount; blendShapeIndex++)
                {
                    var blendShapeName = mesh.GetBlendShapeName(blendShapeIndex);
                    var weight = renderer.GetBlendShapeWeight(blendShapeIndex);

                    var animParam = new AnimParam("", blendShapeName, weight);
                    animParamList.Add(animParam);
                }
            }

            return animParamList;
        }

        /// <summary>
        /// originClipに設定されたAnimationKeyをすべてtargetclipにコピーする
        /// </summary>
        public void CopyAnimationKeysFromOriginClip(AnimationClip originClip, AnimationClip targetClip)
        {
            foreach (var binding in AnimationUtility.GetCurveBindings(originClip))
                AnimationUtility.SetEditorCurve(targetClip, binding, AnimationUtility.GetEditorCurve(originClip, binding));
        }
    }
}
