[System.Serializable]
public class GameData
{
    /// <summary> 게임 데이터 기준 </summary>
    /// 플레이어가 두 명 이상이라고 가정한 뒤
    /// 그들이 공유하지 않고 독자적으로 가져야하는 정보
    /// ...라고 생각되는 것들을 넣었습니다
    /// 
    /// 현재 라운드 / 현재 적의 수 같은 정보는 스테이지 매니저에 있음!!
    /// 혹시 수정해야 할 것 같으면 편하게 말씀해주세요!

    //최대 생명 (시작 생명)
    public int maxLife;

    //현재 생명
    public int life;

    //보유 소환석 (구 골드)
    public int summonToken;

    //보유 강화석
    public int upgradToken;

    //전리품(구 선택권)
    public int rewardTicket;

    public GameData(int maxLife, int life, int summonToken = 0, int upgradToken = 0, int rewardTicket = 0)
    {
        this.maxLife = maxLife;
        this.life = life;
        this.summonToken = summonToken;
        this.upgradToken = upgradToken;
        this.rewardTicket = rewardTicket;
    }
}
