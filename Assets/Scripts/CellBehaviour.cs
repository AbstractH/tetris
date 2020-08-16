using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class CellBehaviour : MonoBehaviour
{
    private ParticleSystem particles;
    private Vector3 position;
    private Quaternion rotation;
    private MeshRenderer renderer;
    public Action OnExplosionFinished;
    public void Explode()
    {
        StopAllCoroutines();
        StartCoroutine("Explosion");
    }

    IEnumerator Vibration()
    {
        StartCoroutine("ChaoticRotation");
        int frames = 3;
        float distance = new Vector3(1f,1f,1f).magnitude - transform.localScale.magnitude;
        distance *= distance;
        Vector3 d = GetOpositeDirection(distance);
        for (int i = 0; i < frames+1; i++)
        {
            transform.position += d / frames;
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine("ChaoticRotation");
        StartCoroutine("Vibration");
    }

    private void MoveToRandomPositionInRange(float d)
    {
        float angle = new Random().Next(0, 360);
        Vector2 np = RotateVector(Vector2.left, angle);
        np *= d;
        transform.position += new Vector3(np.x,np.y,transform.position.z);
    }
    
    public static Vector2 RotateVector(Vector2 v, float degrees) {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = v.x;
        float ty = v.y;
 
        return new Vector2(
            cos * tx - sin * ty, 
            sin * tx + cos * ty
        );
    }
    
    private Vector3 GetOpositeDirection(float distance)
    {
        int range = 60;
        Vector3 OC = transform.position;
        Vector3 OP = position;
        Vector3 CP = OP - OC;
        Vector2 PD = RotateVector(new Vector2(CP.x,CP.y), new Random().Next(-range,range));
        Vector3 CD = CP + new Vector3(PD.x,PD.y,OP.z).normalized * distance;
        return CD;
    }

    private Vector3 RandomVector()
    {
        Random random = new Random();
        return new Vector3(random.Next(-360, 360), random.Next(-360, 360), random.Next(-360, 360)).normalized;
    }
    
    IEnumerator ChaoticRotation()
    {
        float angle = 8;
        int frames = 6;
        Quaternion r = transform.rotation;
        Vector3 rd = RandomVector()*angle;
       
        while (true)
        {
            r.eulerAngles += (rd / frames);
            transform.rotation = r;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator Explosion()
    {
        MoveToRandomPositionInRange(0.1f);
        StartCoroutine("Vibration");
        while (transform.localScale.magnitude > new Vector3(1f,1f,1f).magnitude*0.8f)
        {
            transform.localScale *= 0.99f;
            yield return new WaitForEndOfFrame();
        }
        while (transform.localScale.magnitude < new Vector3(1f,1f,1f).magnitude*1.1f)
        {
            transform.localScale *= 1.08f;
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine("Vibration");
        StopCoroutine("ChaoticRotation");
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = new Vector3(1f,1f,1f);
        renderer.enabled = false;
        particles.Play();
        yield return new WaitForSeconds(3f);
        renderer.enabled = true;
    }

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        position = transform.position;
        rotation = transform.rotation;
    }

    void Update()
    {
        
    }
}
