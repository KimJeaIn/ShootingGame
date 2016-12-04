using UnityEngine;
using System.Collections;
using System;

// 적을 자동으로 추격하는 플레이어의 미사일 클래스
public class AutoTargetMissile_Player : Missile {   

    // 적에게 명중했을때 발동. 특별한 경우가아니면 모든 발사체의 공통이며 tag가 플레이어, 적의 차이만있다.
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!m_HitMissile)
            {
                // 데미지만큼 점수를 올린다.
                BoomCounting.Instance.Score(Damage);
                // 발사체가 명중했을때 발동하는 코루틴.
                StartCoroutine("MissileHit");
            }
        }
    }

    // 다른 발사체와 다르게 초기화후 적을 찾기위한 코루틴을 시작한다.
    public override void MissileInit(Vector3 pos, Vector3 dir)
    {
        // 기존 발사체 초기화
        base.MissileInit(pos, dir);
        
        StartCoroutine("TargetLookOn");
    }

    // 타겟 찾기위한 코루틴
    IEnumerator TargetLookOn()
    {
        // 미사일이 날아가는 정면을 바라보게한다.
        this.transform.forward = Rigid.velocity;

        // 바로 타겟을 찾지않기위한 딜레이
        yield return new WaitForSeconds(0.7f);

        EnemyLife target = null;
        float dis = 0f;

        // 적의 목록중 현재 활성화된 적을 찾고 그중 가장 거리가 가까운 타겟을 저장한다.
        for(int i = 0; i < EnemyManager.Instance.EnemyPosList.Count; i++)
        {
            if(EnemyManager.Instance.EnemyPosList[i].EnemyObject.activeSelf)
            {
                // 적과 발사체의 거리를 저장하기위한 변수
                float currentdis = Vector3.Distance(EnemyManager.Instance.EnemyPosList[i].transform.position, this.transform.position);

                // 가장 가까운 적을 target변수에 저장하고 더 가까운 적이있으면 갱신한다.
                if((dis == 0f || dis > currentdis) && EnemyManager.Instance.EnemyPosList[i].Life > 0)
                {
                    target = EnemyManager.Instance.EnemyPosList[i];
                    dis = currentdis;
                }
            }
        }       

        // 타겟을 찾아서 저장했을경우 발동
        if(target != null)
        {
            Transform targetpos = target.transform;

            // 타겟의 위치를 계속 갱신하며 날라가는 방향을 바꾼다.
            while (!m_HitMissile)
            {
                // 타겟에게 부드럽게 다가가기위해서 Vector3.Lerp를 사용했다.
                Rigid.velocity = Vector3.Lerp(Rigid.velocity.normalized, (targetpos.position - this.transform.position).normalized, 0.2f) * Speed;
                this.transform.forward = Rigid.velocity;

                // 타겟이 죽거나 사라질때까지 추격
                if (target.Life <= 0 || !target.gameObject.activeSelf)
                    break;

                yield return 0;
            }
        }

        yield return 0;
    }
}
