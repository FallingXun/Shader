using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationEditor : Editor
{
    [MenuItem("Assets/CustomAnimation/优化曲线精度")]
    static void OptionalFloatCurves()
    {
        // 保留两位小数，看需求定义
        var floatFormat = "f2";
        // 需要直接选中AnimationClip
        var animation_go = Selection.activeObject;
        if (animation_go.GetType() == typeof(AnimationClip))
        {
            // clip
            var clip = animation_go as AnimationClip;
            // 获取Animation的所有Curve
            var binds = AnimationUtility.GetCurveBindings(clip);
            foreach (var bind in binds)
            {
                var curve = AnimationUtility.GetEditorCurve(clip, bind);
                var keys = curve.keys;
                for (int index = 0; index < keys.Length; index++)
                {
                    var keyframe = keys[index];
                    keyframe.value = float.Parse(keyframe.value.ToString(floatFormat));
                    keyframe.inTangent = float.Parse(keyframe.inTangent.ToString(floatFormat));
                    keyframe.outTangent = float.Parse(keyframe.outTangent.ToString(floatFormat));

                    keys[index] = keyframe;
                }
                // struct 需要重新指定
                curve.keys = keys;
                // 重新指定
                AnimationUtility.SetEditorCurve(clip, bind, curve);
            }
            EditorUtility.SetDirty(clip);
            // 重新保存
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Assets/CustomAnimation/优化曲线精度1")]
    public static void CompressAnimationClip()
    {
        var animation_go = Selection.activeObject;
        var clip = animation_go as AnimationClip;
        AnimationClipCurveData[] tCurveArr = AnimationUtility.GetAllCurves(clip);
        Keyframe tKey;
        Keyframe[] tKeyFrameArr;
        for (int i = 0; i < tCurveArr.Length; ++i)
        {
            AnimationClipCurveData tCurveData = tCurveArr[i];
            if (tCurveData.curve == null || tCurveData.curve.keys == null)
            {
                continue;
            }
            tKeyFrameArr = tCurveData.curve.keys;
            for (int j = 0; j < tKeyFrameArr.Length; j++)
            {
                tKey = tKeyFrameArr[j];
                tKey.value = float.Parse(tKey.value.ToString("f2")); //#.### 
                tKey.inTangent = float.Parse(tKey.inTangent.ToString("f2"));
                tKey.outTangent = float.Parse(tKey.outTangent.ToString("f2"));
                tKeyFrameArr[j] = tKey;
            }
            tCurveData.curve.keys = tKeyFrameArr;
            clip.SetCurve(tCurveData.path, tCurveData.type, tCurveData.propertyName, tCurveData.curve);
        }
    }

    [MenuItem("Assets/CustomAnimation/优化曲线精度2")]
    public static void CompressAnimationClip_2()
    {
        var animation_go = Selection.activeObject;
        var clip = animation_go as AnimationClip;
        AnimationClipCurveData[] tCurveArr = AnimationUtility.GetAllCurves(clip);
        Keyframe tKey;
        Keyframe[] tKeyFrameArr;
        for (int i = 0; i < tCurveArr.Length; ++i)
        {
            AnimationClipCurveData tCurveData = tCurveArr[i];
            if (tCurveData.curve == null || tCurveData.curve.keys == null)
            {
                continue;
            }
            tKeyFrameArr = tCurveData.curve.keys;
            for (int j = 0; j < tKeyFrameArr.Length; j++)
            {
                tKey = tKeyFrameArr[j];
                tKey.value = float.Parse(tKey.value.ToString("f3")); //#.### 
                tKey.inTangent = float.Parse(tKey.inTangent.ToString("f3"));
                tKey.outTangent = float.Parse(tKey.outTangent.ToString("f3"));
                tKeyFrameArr[j] = tKey;
            }
            tCurveData.curve.keys = tKeyFrameArr;
            clip.SetCurve(tCurveData.path, tCurveData.type, tCurveData.propertyName, tCurveData.curve);
        }
    }
}
