using UnityEngine;
using System.Collections;

// 플레이어, 적 발사체의 부모가되는 클래스.
public abstract class Missile : MonoBehaviour
{
    // 발사체가 명중했을대의 데미지
    public int Damage = 1;
    // 발사체의 강체
    public Rigidbody Rigid;
    // 발사체의 속도
    public float Speed = 1f;

    // 발사체가 날아갈때의 이펙트
    public GameObject EffectObject;
    // 발사체가 파괴될때 이펙트
    public GameObject DestroyEffectObject;

    // 발사체가 파괴될때 사운드
    public AudioSource ExpSound;

    // 발사체가 아무것도 맞추지 못했을때 초기화되기위한 시간
    public float DestroyTime = 3f;

    // 발사체가 누군가 맞췄을때 true가 된다
    protected bool m_HitMissile = false;
    protected float m_tick = 0f;
    // 이펙트가 바로 사라지는것을 막기위한 딜레이
    protected float m_effectDestroyTime = 0.5f;

    void Update()
    {
        // 발사체가 아무도 맞추지 못했을경우 발사체가 사라지기위한 부분
        if (!m_HitMissile)
        {
            m_tick += Time.deltaTime;

            if (m_tick > DestroyTime)
            {
                m_tick = 0f;
                EndMissile();
            }
        }
    }
    // 발사체를 발사할때 쓰이는 함수
    public virtual void MissileInit(Vector3 pos, Vector3 dir)
    {
        transform.position = pos;
        Rigid.velocity = dir * Speed;
    }
    // 발사체가 누군가에게 맞았을경우 발동하는 코루틴. 플레이어, 적의 발사체를 다루기위해 OnTriggerEnter는 자식클래스에 넣었다.
    public virtual IEnumerator MissileHit()
    {
        m_HitMissile = true;

        if (ExpSound != null)
            ExpSound.enabled = true;

        DestroyMissile();

        // 폭발 이펙트가 바로 사라지는것을 방지하기위해 일정시간 딜레이를 줬다.
        yield return new WaitForSeconds(m_effectDestroyTime);

        if (ExpSound != null)
            ExpSound.enabled = false;

        m_HitMissile = false;
        EndMissile();
    }
    // 발사체가 누군가에게 맞아서 폭발할때 함수
    public virtual void DestroyMissile()
    {
        Rigid.velocity = Vector3.zero;

        EffectObject.SetActive(false);
        DestroyEffectObject.SetActive(true);
    }
    // 폭발하거나 시간이 다 된 발사체를 사라지게하는 함수
    public virtual void EndMissile()
    {
        Rigid.velocity = Vector3.zero;
        m_tick = 0f;

        EffectObject.SetActive(true);
        DestroyEffectObject.SetActive(false);

        this.gameObject.SetActive(false);
    }
}