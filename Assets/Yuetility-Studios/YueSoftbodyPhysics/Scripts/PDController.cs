//  Written by Marcel Remmers © for Yuetility 10.06.22
using UnityEngine;
using YuetilitySoftbody;

namespace YuetilitySoftbody
{
    public class PDController
    {
        private Vector3 PD;

        private Vector3 LastE;
        private Vector3 P;
        private Vector3 D;

        public Vector3 CalculatePD(Vector3 e, float pk, float dk)
        {
            P = e * pk;
            D = ((e - LastE) / Time.fixedDeltaTime) * dk;



            PD = P + D;

            LastE = e;

            return PD;
        }
        public Vector3 CalculatePD(Vector3 e, float pk, float dk, bool b)
        {
            P = e * pk;
            D = ((e - LastE) / Time.fixedDeltaTime) * dk;


            PD = P + D;

            LastE = e;

            return PD;
        }
    }
}
