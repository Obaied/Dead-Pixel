using UnityEngine;

public class MapPlane : MonoBehaviour
{
    private GameObject m_Player = null;
    private bool m_Activated = false;


    //Unity Functions

    void Start()
    {
        GetComponent<Renderer>().enabled = false;
        m_Player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (m_Activated || m_Player == null)
            return;

        Vector3 vToPlayer = m_Player.transform.position - transform.position;
        vToPlayer.y = 0;

        if (vToPlayer.sqrMagnitude < 15f)
        {
            GetComponent<Renderer>().enabled = true;
            m_Activated = true;
        }
    }
}