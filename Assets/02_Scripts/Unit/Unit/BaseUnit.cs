using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnit : MonoBehaviour
{
    private IRun running;
    private IRecovery recovery;
    private UnitAnimationHanlder animationHandler;

    private Action? onRecovery;         // 회복 시
    private Action? onReach;            // 도착시
    private Action? onDieAction;        // 죽을 때 

    [Header("===Change State===")]
    [SerializeField] private float unitHp;              // 변경되는 HP
    [SerializeField] private float unitSpeed;           // 변경되는 속도

    [SerializeField] private UnitData unitData;         // 변경되지 않는 데이터 

    [SerializeField] private bool isDeadState;

    [Header("===Conponent===")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider unitCollder;
    [SerializeField] private UnitHpBar unitHpBar;

    public UnitData UnitData { get => unitData; }
    public float Hp { get => unitHp; set { unitHp = value; } }
    public bool IsDeadState { get => isDeadState; }
    public UnitHpBar UnitHpBar { get => unitHpBar;  }
    public float UnitSpeed { get => unitSpeed; }

    private void Awake()
    {
        // 액션 초기화
        SetUpEventAction();

        animator = gameObject.GetComponent<Animator>();
        unitCollder = gameObject.GetComponent<Collider>();
        unitHpBar = gameObject.GetComponent<UnitHpBar>();
        animationHandler = new UnitAnimationHanlder(animator);
    }

    private void Start()
    {
        isDeadState = false;

        // Hp바 업데이트 
        UpdatetHpBarUi();

        // run 실행 
        if (running != null)
        {
            running.IRun(EnemyManger.Instance.point, onReach);
        }

        // recovery 실행(회복안하면 실행x)
        if (recovery != null && unitData.recoverRate > 0) 
        {
            recovery.IRecovery(onRecovery);
        }
    }

    #region 인터페이스 / 데이터 초기화
    public void InitRunning(IRun run) 
    {
        this.running = run;
    }
    public void InitRecovery(IRecovery recovery) 
    {
        this.recovery = recovery;
    }
    public void InitUnitdata(UnitData data) 
    {
        this.unitData = data;

        this.unitHp = data.maxHp;
        this.unitSpeed = data.speed;
    }
    #endregion

    private void SetUpEventAction() 
    {
        // 죽을 때 실행 
        // 1. 삭제 
        onDieAction = () =>
        {
            this.StopAllCoroutines();   // 현재 Mono에서 파생된 모든 코루틴 종료 (UnitRun/Recovery)
            DieAndDestroy();
            DeleteCollider();
            DeliveryIndex(true);
            SoundManager.Instance.SFXPlay(SFXType.MonsterDie, transform.position);
        };

        // 회복시 실행
        // 1. hp바 업데이트 2. 회복 파티클 실행 
        onRecovery = () =>
        {
            UpdatetHpBarUi();
        };

        // 도착시 실행
        // 1. 삭제
        // 2. 플레이어 hp 깎기
        onReach = () => 
        {
            Destroy(gameObject);
            ReducePlayerHp();
            DeliveryIndex(false);   
        };
    }

    private void ChangeUnitAnimation(EnemyAnimationType type)
    {
        animationHandler.ChangeAnimation(type);
    }

    public void DieAndDestroy()
    {
        // die 애니메이션 실행
        ChangeUnitAnimation(EnemyAnimationType.Die);

        StartCoroutine(RunAniWait());
    }

    IEnumerator RunAniWait() 
    {
        // 애니메이션 실행될 때까지 기다리기
        // die는 HasExitTime 해제
        yield return new WaitUntil(()=> animator.GetCurrentAnimatorStateInfo(0).IsName("Death"));

        while (true) 
        {
            float time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            // 90퍼정도 실행되었을 때 종료
            if (time >= 0.9f)
                break;

            // 한 프레임 대기 
            yield return null;
        }

        yield return new WaitForSeconds(0.02f);
        Destroy(gameObject);
    }

    public void UpdatetHpBarUi() 
    {
        unitHpBar.UpdateHpBar(this.unitHp / unitData.maxHp);
    }

    private void DeleteCollider() 
    {
        Destroy(unitCollder);
    }

    #region 상호작용

    public void GetDamage(float damage) 
    {
        // 죽은 상태이면 
        if (isDeadState) 
            return;

        this.unitHp -= damage;

        // hp 바 업데이트 
        UpdatetHpBarUi();

        if (unitHp <= 0) 
        {
            isDeadState = true;

            // 사망 액션 실행
            onDieAction?.Invoke();
        }
    }

    public void SlowEnemy(float rate) 
    {
        // 감소 비율
        this.unitSpeed = unitData.speed * rate;
    }

    public void DoneSlowEnemy() 
    {
        this.unitSpeed = unitData.speed;
    }

    #endregion


    private void ReducePlayerHp() 
    {
        // 공격력만큼 데미지를 준다.
        UserManager.Instance.TakeDamage(unitData.attack);
    }

    private void DeliveryIndex(bool isDead)
    {
        // 스테이지 매니저에게 사라진 몬스터의 num 전달
        StageManager.Instance.SendIndex(unitData.num, isDead);
    }  

}