using System;
using UnityEngine;

public class UserManager : Singleton<UserManager>
{
    public DifficultyType difficulty;   //난이도
    public GameData gameData;

    // gameData 변경 시 호출되는 액션
    public Action onRewardTicketChange;  // 전리품 선택권 변경 시 호출
    public Action onSummonTokenChange;   // 소환석 변경 시 호출
    public Action onUpgradTokenChange;   // 강화석 변경 시 호출
    public Action onLifeChange;          // 생명 변경 시 호출

    // 현재 생명
    public int Life
    {
        get
        {
            return gameData.life;
        }

        set
        {
            gameData.life = value;

            if (gameData.life <= 0)
            {
                gameData.life = 0;
                Debug.Log("현재 생명: " + gameData.life);
                onLifeChange?.Invoke();
                StageManager.Instance.GameOver();
            }
            else
            {
                Debug.Log("현재 생명: " + gameData.life);
                onLifeChange?.Invoke();
            }
        }
    }

    // 보유 소환석 (구 골드)
    public int SummonToken
    {
        get
        {
            return gameData.summonToken;
        }
        set
        {
            gameData.summonToken = value;

            if(gameData.summonToken < 0)
            {
                gameData.summonToken = 0;
            }
            else
            {
                Debug.Log("현재 소환석: " + gameData.summonToken);
                onSummonTokenChange?.Invoke();
            }
        }
    }

    // 보유 강화석
    public int UpgradToken
    {
        get
        {
            return gameData.upgradToken;
        }
        set
        {
            gameData.upgradToken = value;
            if(gameData.upgradToken < 0)
            {
                gameData.upgradToken = 0;
            }
            else
            {
                Debug.Log("현재 강화석: " + gameData.upgradToken);
                onUpgradTokenChange?.Invoke();
            }
        }
    }

    // 전리품(구 선택권)
    public int RewardTicket
    {
        get
        {
            return gameData.rewardTicket;
        }
        set
        {
            // 0보다 작은 값이 들어오면 return
            if (value < 0) return;


            gameData.rewardTicket = value;
            Debug.Log("현재 전리품 선택권: " + gameData.rewardTicket);
            onRewardTicketChange?.Invoke();
        }
    }

    // 초기화
    protected override void Initialize()
    {
        int userlife = DataStore.stageLevelDataList[0].maxLife;
        gameData = new GameData(userlife, userlife);
    }

    // 게임 데이터 초기화
    public void RestartGameData()
    {
        Life = gameData.maxLife;
        UpgradToken = 0;
        SummonToken = 200;
        RewardTicket = 0;
    }


    // 전리품 선택권이 있는지 확인하는 메서드
    public bool HasRewardTicket()
    {
        if (RewardTicket <= 0)
        {
            Debug.Log("사용 가능한 선택권이 없습니다.");
            UIManager.Instance.GetMessageUI<ErrorPopup>(MessageUIType.ErrorMessage)
                .OnMessage(ErrorType.RewardTicketError);
            return false;
        }
        return true;
    }

    // 충분한 소환석이 있는지 확인하는 메서드
    public bool HasEnoughSummonToken(int amount)
    {
        if (amount > SummonToken)
        {
            Debug.Log("소환석이 부족합니다.");
            UIManager.Instance.GetMessageUI<ErrorPopup>(MessageUIType.ErrorMessage)
                .OnMessage(ErrorType.SummonTokenError);
            return false;
        }

        return true;
    }

    // 충분한 강화석이 있는지 확인하는 메서드
    public bool HasEnoughUpgradToken(int amount)
    {
        if (amount > UpgradToken)
        {
            Debug.Log("강화석이 부족합니다.");
            UIManager.Instance.GetMessageUI<ErrorPopup>(MessageUIType.ErrorMessage)
                .OnMessage(ErrorType.UpgradTokenError);
            return false;
        }
        return true;
    }

    // 소환석 사용 메서드
    public void UseSummonToken(int amount)
    {
        if(HasEnoughSummonToken(amount))
        {
            SummonToken -= amount;
        }   
    }

    // 강화석 사용 메서드
    public void UseUpgradToken(int amount)
    {
        if (HasEnoughUpgradToken(amount))
        {
            UpgradToken -= amount;
        }
    }

    // 소환석 추가 메서드
    public void AddSummonToken(int amount)
    {
        SummonToken += amount;
        SoundManager.Instance.SFXPlay(SFXType.AddSummonToken);
    }

    // 강화석 추가 메서드
    public void AddUpgradToken(int amount)
    {
        UpgradToken += amount;
        SoundManager.Instance.SFXPlay(SFXType.AddEnhanceToken);

    }

    // 플레이어가 공격 받았을 때 생명 감소
    public void TakeDamage(int amount)
    {
        if (Life <= 0) return;

        Life -= amount;
    }

    // 소환석을 강화석으로 교환
    public bool SummonToUpgrade(out int token)
    {
        token = 0;
        if (!HasEnoughSummonToken(100)) 
            return false;

        SummonToken -= 100;

        int percent = UnityEngine.Random.Range(0, 100);
        if (percent >= 0 && percent < 33)
        {
            Debug.Log("소환석 100개 <-> 강화석 100개");
            AddUpgradToken(100);
            token = 100;
        }
        else if (percent >= 33 && percent < 66)
        {
            Debug.Log("소환석 100개 <-> 강화석 50개");
            AddUpgradToken(50);
            token = 50;
        }
        else if (percent >= 66 && percent < 100)
        {
            Debug.Log("소환석 100개 <-> 강화석 30개");
            AddUpgradToken(30);
            token = 30;
        }

        return true;
    }
}
