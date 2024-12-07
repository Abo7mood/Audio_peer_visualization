//  Written by Marcel Remmers © for Yuetility 10.06.22
using UnityEngine;
using YuetilitySoftbody;

namespace YuetilitySoftbody
{
    [System.Serializable]
    public class PDTuning
    {
        [HideInInspector]
        public Transform TransTraget;
        [HideInInspector]
        public Vector3 Tensor = Vector3.one * 0.1f;

        public float PositionProportional = 100f;
        public float PositionDerivative = 10f;

        [HideInInspector]
        public float RotationProportional = 1f;
        [HideInInspector]
        public float RotationDerivative = 0.1f;
        [HideInInspector]
        public float maxDepenetrationVelocity = 1f;
    }
}
