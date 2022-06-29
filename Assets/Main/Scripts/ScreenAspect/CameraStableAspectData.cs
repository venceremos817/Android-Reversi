using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraStableAspectData", menuName = "ScriptableObjects/CameraStableAspectData")]
public class CameraStableAspectData : ScriptableObject
{
    // ���̃X�N���v�^�u���I�u�W�F�N�g���ۑ����Ă���ꏊ�̃p�X
    public const string PATH = "ScriptableObjects/CameraStableAspectData";
    private static CameraStableAspectData _entity;
    public static CameraStableAspectData Entity
    {
        get
        {
            // ���A�N�Z�X���Ƀ��[�h����
            if (_entity == null)
            {
                _entity = Resources.Load<CameraStableAspectData>(PATH);
                // ���[�h�ł��Ȃ������ꍇ�̓G���[���O
                if (_entity == null)
                    Debug.LogError(PATH + "not found");
            }
            return _entity;
        }
    }



    /// <summary>
    /// �𑜓x
    /// </summary>
    [System.Serializable]
    public class TargetResolution
    {
        [Min(0.0f)]
        public int width = 360;
        [Min(0.0f)]
        public int height = 640;

        public float AspectRatio { get => (float)width / height; }
    }

    [SerializeField]
    private TargetResolution _targetResolution;
    [SerializeField]
    private float _pixelPerUnit = 100.0f;

    public TargetResolution targetResolution { get => _targetResolution; }
    public float pixelPerUnit { get => _pixelPerUnit; }
}
