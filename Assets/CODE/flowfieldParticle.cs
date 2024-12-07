using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowfieldParticle : MonoBehaviour
{
    Rigidbody rb; //physics
    [Header("Varbiles")]
    public float frequency; //varible we create just to adjust the size of the particle system
    public float _moveSpeed; 
    public int _audioBand;
    public float maxSpeed;
    public GameObject explosionPrefabHorizontal; //crack particle
    public GameObject explosionPrefabVertical; //crack particle
    GameObject explosion; //the prefab
    private float crackTime=3f; // destroy time 
    AudioPeer audioPeer;
   [Header("Sound Data")]
    public AudioSource audioSource;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioPeer = FindObjectOfType<AudioPeer>();
        audioSource = audioPeer.GetComponent<AudioSource>();
        if (!audioSource)
        {
            Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];
    }

   

    // Use this for initialization
    
    // Update is called once per frame
    void Update()
    {

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
        }
        Debug.Log($"ClipLoudness = {clipLoudness}");
    }
    // Update is called once per frame
    void FixedUpdate() //physics update
    {
        
      
        rb.AddForce(transform.forward * _moveSpeed * Time.deltaTime);

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public void ApplyRotation(Vector3 rotation, float rotateSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(rotation.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player")) // player = bulid
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 pos = contact.point; // position
            if (transform.position.y < 20)
            {
                 explosion = Instantiate(explosionPrefabHorizontal, pos, Quaternion.identity); //create particle system (crack)
            }
            else
            {
                 explosion = Instantiate(explosionPrefabVertical, pos, Quaternion.identity); //create particle system (crack)

            }
            //getting the size
            var main = explosion.GetComponent<ParticleSystem>().main; 
            main.startSize = Size()*10;
            //destroy
            Destroy(explosion, crackTime);
        }
        
    }

    private float Size()
    {
        
        float size = clipLoudness * frequency;
        float max_Y_AXIS = 600; // maximum height 
        float yAxis = transform.position.y;
        float yAxisFix = yAxis / max_Y_AXIS;
        float yAxisFinal = yAxisFix *100;
        float finalSize = size / yAxisFinal;
        if (finalSize > 3) finalSize = 3;
        return finalSize;
    }
}
