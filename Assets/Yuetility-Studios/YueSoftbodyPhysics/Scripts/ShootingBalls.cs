using UnityEngine;
using YuetilitySoftbody;

namespace YuetilitySoftbody
{
    public class ShootingBalls : MonoBehaviour
    {
        [SerializeField]
        private float mouseSpeed = 200f;
        [SerializeField]
        private float shootingSpeed = 25f;
        [SerializeField]
        private GameObject ballPrefab;

        private float counter = 1f;

        void Start()
        {

        }

        void Update()
        {
            if (Input.GetButton("Fire1") && counter <= 0)
            {
                GameObject temp = Instantiate<GameObject>(ballPrefab, transform.position, Quaternion.Euler(0, 0, 0));

                temp.GetComponent<Rigidbody>().velocity = transform.forward * shootingSpeed;
                Destroy(temp, 5f);
                counter = 0.1f;
            }

            transform.Rotate(transform.right * mouseSpeed * -Input.GetAxis("Mouse Y") * Time.deltaTime);
            transform.Rotate(transform.up * mouseSpeed * Input.GetAxis("Mouse X") * Time.deltaTime);

            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);


            counter -= Time.deltaTime;

            if (counter < 0)
                counter = 0;
        }
    }
}