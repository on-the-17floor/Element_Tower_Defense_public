using UnityEditor;
using UnityEngine;

// 커스텀 에디터 윈도우
// : EditorWindow을 상속 
public class TestEditorWindow : EditorWindow
{
    /// <summary>
    /// 
    /// EditorWindow도 ScriptableObject임
    /// 에셋으로 저장도 가능 (현재 구현 x)
    /// 
    /// </summary>

    private enum TabType
    {
        EnemyTab,
        StageTab,
        EctTab
    }

    // 커스텀 윈도우 에디터 
    static TestEditorWindow exampleWindow;

    // Type
    static private TabType tabType = TabType.EnemyTab;

    // Texture
    static private Texture banner;

    // Enemy Tab
    static private int enemyNum = -1;

    // Menu생성 : Window 하위의 Test 란 이름으로
    [MenuItem("Element/Test", false, 1)]
    static void Open()
    {
        #region Window 표시방법

        // 다수의 EditorWindow 가능
        /*
        // EditorWindow 생성
        exampleWindow = CreateInstance<TestEditorWindow>();
        // 에디터 표시 
        exampleWindow.Show();
        */

        // 단일 EditorWindow
        // 방법1. null 검사 후 create
        /*
        if (exampleWindow == null) 
        {
            exampleWindow = CreateInstance<TestEditorWindow>();
        }
        exampleWindow.Show();
        */
        #endregion

        // 방법2. GetWindow 사용 
        // GetWindow : 이미 존재하면 해당 인스턴스 가져옴, 없으면 생성 후 Show
        exampleWindow = GetWindow<TestEditorWindow>();
        // 최소,최대크기 정하기 (사이즈변경 불가능하게)
        exampleWindow.maxSize = exampleWindow.minSize = new Vector2(350, 600);
    }

    // 커스텀 윈도우에 표시되는건 모두 이 하위에 
    private void OnGUI()
    {
        // OnGUI는 매 프레임 실행

        // 인스턴스 실행 전까지는 인스펙터창에 표시 x 
        if (EnemyManger.Instance == null)
            return;
        if (StageManager.Instance == null)
            return;

        float temp = EditorGUIUtility.currentViewWidth / 3;
        GUILayoutOption[] style = { GUILayout.Width(temp), GUILayout.Height(25) };

        // type 버튼 표시
        // 하위 요소를 수평으로 표현
        GUILayout.BeginHorizontal();
        // 몬스터 탭 버튼
        if (GUILayout.Button("EnemyTab", style))
        {
            tabType = TabType.EnemyTab;
        }
        // Stage 탭 버튼
        if (GUILayout.Button("StageTab" , style))
        {
            tabType = TabType.StageTab;
        }
        // ECT 탭 버튼
        if (GUILayout.Button("EctTab" , style))
        {
            tabType = TabType.EctTab;
        }
        GUILayout.EndHorizontal();

        // 이미지 넣기 
        EditorGUILayout.BeginVertical("box");

        // Resources 폴더에서 이미지 로드
        Texture2D logoTexture = Resources.Load("Banner/ElementalTowerDefenceImage") as Texture2D;
        if (logoTexture != null)
        {
            float width = EditorGUIUtility.currentViewWidth - 20; // 가로로 꽉 채우기 여백20

            Rect rect = GUILayoutUtility.GetRect(width, 100f);
            GUI.DrawTexture(rect, logoTexture, ScaleMode.StretchToFill);

            // GUIContent를 사용하여 텍스처 표시
            // GUIContent logoContent = new GUIContent(logoTexture);
            // GILayout.Label(logoContent, GUILayout.MaxWidth(400f), GUILayout.MaxHeight(100f));
        }
        else
        {
            EditorGUILayout.HelpBox("Banner 이미지를 찾을 수 없습니다. Resources 폴더에 존재하는지 확인하세요.", MessageType.Warning);
        }
        EditorGUILayout.EndVertical();

        // Type에 따라 출력 
        if (tabType == TabType.EnemyTab)
        {
            DrawEnemyTabContenct();
        }
        else if (tabType == TabType.StageTab)
        {
            DrawStageTabContenct();
        }
        else if (tabType == TabType.EctTab) 
        {
            DrawEctTabContenct();
        }
    }

    private void DrawEnemyTabContenct()
    {
        // 한줄띄우기
        EditorGUILayout.Space();

        // 현재 선택된 enemy 표시
        EditorGUILayout.LabelField($"현재 선택된 Enemy : {(enemyNum >= 0 ? enemyNum.ToString() : "선택안됨")} ");

        EditorGUILayout.LabelField("사용가능한 적 목록");
        // 에디터에 텍스를 띄움 : GUILayout.Label(""); 와 같다
        foreach (var enemy in EnemyManger.Instance.NumToData)
        {
            int idx = enemy.Key;
            UnitData unitData = enemy.Value;

            // 선택된 버튼 색 변경
            GUI.backgroundColor = (idx == enemyNum) ? Color.green : Color.white;

            // 인덱스에 맞는 몬스터 생성
            if (GUILayout.Button($"{unitData.name}"))
            {
                // 클릭 시 
                // 인덱스 저장 
                enemyNum = idx;

                // Debug.Log($"{idx} 버튼 누름");
            }
        }

        // 하위 버튼까지 영향 안 미치게
        GUI.backgroundColor = Color.white;

        // 몬스터 생성 버튼
        if (GUILayout.Button("Create Enemy"))
        {
            if (enemyNum < 0)
                return;

            EnemyManger.Instance.GetEnemy(enemyNum);
        }
    }

    private void DrawStageTabContenct() 
    {
        
    }

    private void DrawEctTabContenct() 
    {
        // 버튼 색 변경 
        GUI.backgroundColor = Color.yellow;

        // Stage 관련 버튼
        if (GUILayout.Button("미션 1 시작"))
        {
            StageManager.Instance.missionList[0].MissionStart();
        }
        if (GUILayout.Button("미션 2 시작"))
        {
            StageManager.Instance.missionList[1].MissionStart(); ;
        }
        if (GUILayout.Button("미션 3 시작"))
        {
            StageManager.Instance.missionList[2].MissionStart();
        }
        if (GUILayout.Button("소환석 전환"))
        {
            //UserManager.Instance.SummonToUpgrade();
        }
        if (GUILayout.Button("소환석 1000 획득"))
        {
            UserManager.Instance.AddSummonToken(1000);
        }
        if (GUILayout.Button("리워드 티켓 증가"))
        {
            UserManager.Instance.RewardTicket++;
        }

        // 하위 버튼까지 영향 안 미치게
        GUI.backgroundColor = Color.white;
    }

}
