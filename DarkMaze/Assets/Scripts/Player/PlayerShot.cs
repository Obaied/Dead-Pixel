using UnityEngine;
using System.Collections;

public class PlayerShot : MonoBehaviour
{
    public float KillTime;

	void Start () 
    {
        Destroy(this.gameObject, KillTime);
	}
	
    public void Init(Vector3 a_StartPos, Vector3 a_EndPos)
    {
        var vDir = a_EndPos - a_StartPos;
        float fScale = vDir.magnitude;
        transform.position = a_StartPos + (vDir.normalized * fScale / 2f);
        //transform.position = a_StartPos;
        transform.forward = vDir.normalized;

        var vScale = transform.localScale;
        vScale.z = fScale;
        transform.localScale = vScale;
    }
}
