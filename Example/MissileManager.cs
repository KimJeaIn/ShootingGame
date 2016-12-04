using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 모든 발사체를 관리하는 매니저. 발사체를 실시간 생성하는건 자원소모가 심하기때문에 미리 생성후 활성화하는 메모리풀 방식으로 사용한다.
public class MissileManager : MonoBehaviour {

    // 어디서든 부를수있게끔 Singleton을 사용했다.
    static private MissileManager m_instance;
    static public MissileManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<MissileManager>();

                if (m_instance == null)
                    Debug.LogError("Not Find Instance!!");
            }

            return m_instance;
        }
    }

    // 각 발사체가 만들어지는 갯수
    static int MissileCount = 100;

    // 만들어지는 발사체의 갯수가 정해졌기때문이 배열을 사용했다.
    // 아군 일반발사체를 관리하기위한 리스트
    public Missile[] PlayerNormalMissileList = new Missile[MissileCount];
    public GameObject PlayerNormalMissilePrefab;

    // 아군 유도발사체를 관리하기위한 리스트
    public Missile[] PlayerAutoMissileList = new Missile[MissileCount];
    public GameObject PlayerAutoMissilePrefab;

    // 적 일반발사체를 관리하기위한 리스트
    public Missile[] EnemyNormalMissileList = new Missile[MissileCount];    
    public GameObject EnemyNormalMissilePrefab;

    // 적 레이저발사체를 관리하기위한 리스트
    public Missile[] EnemyLaserMissileList = new Missile[MissileCount];
    public GameObject EnemyLaserMissilePrefab;

    // 보스 발사체를 관리하기위한 리스트
    public Missile[] BossMissileList = new Missile[MissileCount];
    public GameObject BossMissilePrefab;

    // 폭탄 리스트
    public BoomMoving[] BoomList = new BoomMoving[5];

    // 모든 적 발사체를 없애기위한 함수
    public void MissileClear()
    {
        for (int i = 0; i < MissileCount; i++)
        {
            if (EnemyNormalMissileList[i].gameObject.activeSelf)
                EnemyNormalMissileList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < MissileCount; i++)
        {
            if (EnemyLaserMissileList[i].gameObject.activeSelf)
                EnemyLaserMissileList[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < MissileCount; i++)
        {
            if (BossMissileList[i].gameObject.activeSelf)
                BossMissileList[i].gameObject.SetActive(false);
        }
    }

    // 처음 초기화하며 각 발사체를 생성한다. 카메라가 회전하는 게임이기때문에 발사체의 transform의 부모는 카메라로 둔다.
    void Awake()
    {
        for(int i = 0; i < MissileCount; i++)
        {
            PlayerNormalMissileList[i] = GameObject.Instantiate(PlayerNormalMissilePrefab).GetComponent<Missile>();
            PlayerNormalMissileList[i].gameObject.SetActive(false);
            PlayerNormalMissileList[i].transform.parent = Camera.main.transform;
        }

        for (int i = 0; i < MissileCount; i++)
        {
            EnemyNormalMissileList[i] = GameObject.Instantiate(EnemyNormalMissilePrefab).GetComponent<Missile>();
            EnemyNormalMissileList[i].gameObject.SetActive(false);
            EnemyNormalMissileList[i].transform.parent = Camera.main.transform;
        }

        for (int i = 0; i < MissileCount; i++)
        {
            EnemyLaserMissileList[i] = GameObject.Instantiate(EnemyLaserMissilePrefab).GetComponent<Missile>();
            EnemyLaserMissileList[i].gameObject.SetActive(false);
            EnemyLaserMissileList[i].transform.parent = Camera.main.transform;
        }        

        for (int i = 0; i < MissileCount; i++)
        {
            PlayerAutoMissileList[i] = GameObject.Instantiate(PlayerAutoMissilePrefab).GetComponent<Missile>();
            PlayerAutoMissileList[i].gameObject.SetActive(false);
            PlayerAutoMissileList[i].transform.parent = Camera.main.transform;
        }

        for (int i = 0; i < MissileCount; i++)
        {
            BossMissileList[i] = GameObject.Instantiate(BossMissilePrefab).GetComponent<Missile>();
            BossMissileList[i].gameObject.SetActive(false);
            BossMissileList[i].transform.parent = Camera.main.transform;
        }
    }

    // 폭탄을 사용하기위한 함수
    public void GetBoom(Vector3 pos)
    {
        for(int i = 0; i < BoomList.Length; i++)
        {
            // 현재 활성화되지않은 폭탄을 사용한다.
            if(!BoomList[i].MainObject.activeSelf)
            {
                BoomList[i].MainObject.SetActive(true);
                BoomList[i].BoomInit(pos);

                break;
            }
        }
    }

    // 자동추적 미사일을 사용하기위한 함수.
    public void GetAutoMissile(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < MissileCount; i++)
        {
            // 현재 활성화되지않은 미사일을 사용한다.
            if (!PlayerAutoMissileList[i].gameObject.activeSelf)
            {
                PlayerAutoMissileList[i].gameObject.SetActive(true);
                PlayerAutoMissileList[i].MissileInit(pos, dir);                

                break;
            }
        }
    }

    // 레이저를 사용하기위한 함수.
    public void GetLaserMissile(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < MissileCount; i++)
        {
            // 현재 활성화되지않은 레이저를 사용한다.
            if (!EnemyLaserMissileList[i].gameObject.activeSelf)
            {
                EnemyLaserMissileList[i].MissileInit(pos, dir);
                EnemyLaserMissileList[i].gameObject.SetActive(true);

                break;
            }
        }
    }

    // 보스의 발사체를 사용하기위한 함수.
    public void GetBossMissile(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < MissileCount; i++)
        {
            // 현재 활성화되지않은 발사체를 사용한다.
            if (!BossMissileList[i].gameObject.activeSelf)
            {
                BossMissileList[i].MissileInit(pos, dir);
                BossMissileList[i].gameObject.SetActive(true);

                break;
            }
        }
    }

    // 플레이어, 적의 기본 발사체를 사용하기위한 함수
    public void GetNormalMissile(Vector3 pos, Vector3 dir, bool player)
    {
        if(player)
        {
            for (int i = 0; i < MissileCount; i++)
            {
                // 현재 활성화되지않은 발사체를 사용한다.
                if (!PlayerNormalMissileList[i].gameObject.activeSelf)
                {
                    PlayerNormalMissileList[i].MissileInit(pos, dir);
                    PlayerNormalMissileList[i].gameObject.SetActive(true);

                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < MissileCount; i++)
            {
                // 현재 활성화되지않은 발사체를 사용한다.
                if (!EnemyNormalMissileList[i].gameObject.activeSelf)
                {
                    EnemyNormalMissileList[i].MissileInit(pos, dir);
                    EnemyNormalMissileList[i].gameObject.SetActive(true);

                    break;
                }
            }
        }
    }
}
