using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    public float followSpeed = 10f;
    public CinemachineImpulseSource CMIS;
    
    void Update()
    {
        if (Target != null)
        {
            Vector3 followPosition = new Vector3(Target.position.x, Target.position.y + 0.7f, -10f);
            transform.position = Vector3.Lerp(transform.position, followPosition, followSpeed * Time.deltaTime);
        }
    }

    public void DoImppulse(float strength)
    {
        CMIS.m_DefaultVelocity = new Vector3(Random.Range(-strength,strength),Random.Range(-strength,0),0);
        CMIS.GenerateImpulse();
    }
}
