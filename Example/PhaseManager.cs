using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// 적이 나오는 순서를 관리하는 클래스
public class PhaseManager : MonoBehaviour {

    // 다음 페이즈로 넘어가는게 시간제한인지 적이 전멸했을때인지를 정하기위해 만듬
    public enum PhaseStyle { Time, Destroy};
    
    [Serializable]
    public struct EnemyPhase
    {
        // EnemyControl -> 적의 이동경로를 지정한 클래스
        public EnemyControl Phase;
        // Phasedelay -> EnemyControl의 적의 초기화가 끝난후 다음 초기화까지 대기시간
        public float Phasedelay;
    }
    [Serializable]
    public struct EnemyPhaseList
    {
        // 페이즈가 시작할때 대기시간
        public float StartPhaseDelay;
        public List<EnemyPhase> PhaseList;
        public PhaseStyle Style;
        // 페이즈 스타일이 시간제한일때 걸리는 시간
        public float NextPhaseDelay;
    }

    public List<EnemyPhaseList> StagePhaseList = new List<EnemyPhaseList>();
    public int Currentphase = 0;

    void Start()
    {
        StartCoroutine("PhaseStart");
    }

    // 각 페이즈마다 대기시간을 간단하게 하기위해서 코루틴을 사용했다
    IEnumerator PhaseStart()
    {
        for(int i = 0; i < StagePhaseList.Count; i++)
        {
            // 현재 페이즈를 알기위해 저장
            Currentphase = i;

            // 첫 페이즈 대기시간
            yield return new WaitForSeconds(StagePhaseList[i].StartPhaseDelay);

            for (int j = 0; j < StagePhaseList[i].PhaseList.Count; j++)
            {
                // 페이즈에 해당하는 적을 초기화. 초기화된적은 이동하기 시작한다.
                StagePhaseList[i].PhaseList[j].Phase.PhaseInit();

                yield return new WaitForSeconds(StagePhaseList[i].PhaseList[j].Phasedelay);
            }

            if (StagePhaseList[i].Style == PhaseStyle.Time)
                yield return new WaitForSeconds(StagePhaseList[i].NextPhaseDelay);
            else if (StagePhaseList[i].Style == PhaseStyle.Destroy)
                yield return StartCoroutine("DestroyCheck");
        }

        yield return 0;        
    }

    IEnumerator DestroyCheck()
    {
        while(true)
        {
            bool active = false;

            // 모든적이 파괴되거나 사라진것을 체크하는 for문
            for(int i = 0; i < EnemyManager.Instance.EnemyPosList.Count; i++)
            {
                if (EnemyManager.Instance.EnemyPosList[i].EnemyObject.activeSelf)
                {
                    active = true;
                    break;
                }
            }

            if (!active)
                break;

            yield return 0;
        }
    }
}
