//  Written by Marcel Remmers © for Yuetility 10.06.22
using UnityEngine;
using YuetilitySoftbody;

namespace YuetilitySoftbody
{
    public class PositionTracker : MonoBehaviour
    {

        public bool Active = true;

        public Rigidbody rigid;
        public bool Local = false;

        public bool TrackRotation = true;
        public bool TrackPosition = true;

        public Vector3 Tensor = Vector3.one * 0.1f;

        public float PositionProportional = 100f;
        public float PositionDerivative = 10f;

        public float RotationProportional = 1f;
        public float RotationDerivative = 0.1f;

        public float maxDepenetrationVelocity = 0f;

        private PDController positionPD;
        private PDController rotationPD;

        public Transform TransTarget;
        public Transform Attach;

        public Vector3 Force = Vector3.zero;

        public void Start()
        {
            positionPD = new PDController();
            rotationPD = new PDController();

            rigid = GetComponent<Rigidbody>();
        }

        public void FixedUpdate()
        {
            if (!rigid)
                rigid = GetComponent<Rigidbody>();

            UpdateTracker();
        }
        public void Multiply(float val)
        {
            PositionProportional = PositionProportional * val;
            RotationProportional = RotationProportional * val;

            PositionDerivative = PositionDerivative * val;
            RotationDerivative = RotationDerivative * val;
        }

        public void InItTuning(PDTuning temp)
        {

            if (temp.TransTraget)
                TransTarget = temp.TransTraget;

            Tensor = temp.Tensor;

            PositionProportional = temp.PositionProportional;
            RotationProportional = temp.RotationProportional;

            PositionDerivative = temp.PositionDerivative;
            RotationDerivative = temp.RotationDerivative;

            maxDepenetrationVelocity = temp.maxDepenetrationVelocity;
        }
        public void InItTuning(PDTuning temp, Transform newTarget)
        {

            TransTarget = newTarget;

            Tensor = temp.Tensor;

            PositionProportional = temp.PositionProportional;
            RotationProportional = temp.RotationProportional;

            PositionDerivative = temp.PositionDerivative;
            RotationDerivative = temp.RotationDerivative;

            maxDepenetrationVelocity = temp.maxDepenetrationVelocity;
        }
        public void UpdateTracker()
        {

            if (TransTarget && Active)
            {
                // Equalize Tensor
                rigid.inertiaTensor = Tensor;
                rigid.maxDepenetrationVelocity = maxDepenetrationVelocity;

                // Position Error
                Vector3 PositionError;

                if (Attach)
                {
                    if (!Local)
                        PositionError = TransTarget.position - Attach.position;
                    else
                        PositionError = TransTarget.localPosition - Attach.localPosition;
                }
                else
                {
                    if (!Local)
                        PositionError = TransTarget.position - transform.position;
                    else
                        PositionError = TransTarget.localPosition - transform.localPosition;
                }


                // Calculate PD
                if (!Local)
                    Force = positionPD.CalculatePD(PositionError, PositionProportional, PositionDerivative);
                else
                    Force = transform.TransformDirection(positionPD.CalculatePD(PositionError, PositionProportional, PositionDerivative));

                // Add Result
                if (TrackPosition)
                {
                    if (Attach)
                    {
                        rigid.AddForceAtPosition(Force * rigid.mass, Attach.position);
                    }
                    else
                    {
                        if (!Local)
                            rigid.AddForce(Force * rigid.mass);
                        else
                            rigid.AddForce(transform.InverseTransformVector(Force) * rigid.mass);
                    }
                }
                // Rotation Error
                Quaternion delta = TransTarget.rotation * Quaternion.Inverse(transform.rotation);
                delta.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

                if (angleInDegree > 180)
                    angleInDegree -= 360;

                Vector3 RotationError = rotationAxis * angleInDegree;

                // Calculate PD
                Vector3 Torque = rotationPD.CalculatePD(RotationError, RotationProportional, RotationDerivative);

                // Add Result
                if (TrackRotation)
                    rigid.AddTorque(Torque);

            }

        }
    }
}
