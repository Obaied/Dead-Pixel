using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public AudioClip m_ExplodeSound;
    public ParticleSystem m_ExplodeParticle;

    private bool m_Active = false;
    private Vector3 m_Velocity = Vector3.zero;
    private float m_Timer = 0f;


    //Unity Functions

    void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    void Update()
    {
        if (m_Active)
        {
            m_Timer += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (!m_Active)
            return;

        Vector3 vPos = transform.position;
        vPos += m_Velocity*Time.deltaTime;
        transform.position = vPos;
    }

    void OnTriggerEnter(Collider a_Other)
    {
        if (a_Other.tag == "Player")
        {
            a_Other.GetComponent<PlayerActions>().TakeDamage(10f);
        }
        if (a_Other.tag != "Enemy")
        {
            AudioSource.PlayClipAtPoint(m_ExplodeSound, transform.position);
            m_ExplodeParticle.Play();
            m_ExplodeParticle.transform.parent = null;
            Destroy(m_ExplodeParticle.gameObject, 5f);
            Destroy(gameObject);
        }
    }


    //public functions

    public void Init(Vector3 a_StartPos, Vector3 a_Dir, float a_Speed)
    {
        transform.position = a_StartPos;
        m_Active = true;
        m_Velocity = a_Dir.normalized*a_Speed;
    }
}