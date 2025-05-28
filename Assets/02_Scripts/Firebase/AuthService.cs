using Firebase.Auth;
using GooglePlayGames;
using System.Threading.Tasks;
using UnityEngine;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using System;

public class AuthService
{
    public Text errorText;

    public void Login()
    {
        //DataStore.userData = new UserData();
        PlayGamesPlatform.Activate();
        LoginGooglePlayGames();
    }


    private void LoginGooglePlayGames()
    {

        PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play games successful.");

                PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
                {
                    Debug.Log("Authorization code: " + code);
                    await SignInWithFirebase(code); // 👉 아래 함수에서 Firebase 인증 처리
                });
            }
            else
                Debug.LogError($"Play Games 로그인 실패: {status}");
        });


        //PlayGamesPlatform.Instance.Authenticate((status) =>
        //{
        //    if (status == SignInStatus.Success)
        //    {
        //        Debug.Log("Login with Google Play games successful.");

        //        PlayGamesPlatform.Instance.RequestServerSideAccess(true, async code =>
        //        {
        //            Debug.Log("Authorization code: " + code);
        //            await SignInWithFirebase(code); // 👉 아래 함수에서 Firebase 인증 처리
        //        });
        //    }
        //    else
        //    {
        //        Debug.LogWarning($"Play Games 로그인 실패: {status}");
        //        if (errorText != null)
        //            errorText.text = $"Play Games 로그인 실패: {status}";
        //    }
        //}); 
    }

    private async Task SignInWithFirebase(string idToken)
    {
        var auth = FirebaseAuth.DefaultInstance;
        var credential = PlayGamesAuthProvider.GetCredential(idToken);

        try
        {
            // Firebase 로그인 (await)
            var newUser = await auth.SignInWithCredentialAsync(credential);
            Debug.LogFormat("Firebase 로그인 성공: {0} ({1}) ({2})",
                            newUser.DisplayName, newUser.Email, newUser.UserId);

            // userData 세팅
            DataStore.userData = new UserData(newUser.UserId, newUser.DisplayName, newUser.Email);

            // DB 조회 및 생성
            await SearchUserData(newUser.UserId);
        }
        catch (Exception e)
        {
            Debug.LogError($"Firebase 로그인 실패: {e}");
            if (errorText != null)
                errorText.text = "Firebase 로그인에 실패했습니다.";
        }
    }

    private async Task SignInWithFirebase2(string idToken)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        Credential credential = PlayGamesAuthProvider.GetCredential(idToken);
        
        
        await auth.SignInWithCredentialAsync(credential).ContinueWith(async task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Firebase 로그인 실패: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase 로그인 성공: {0} ({1}) ({2})", newUser.DisplayName, newUser.Email, newUser.UserId);
            DataStore.userData = new UserData(newUser.UserId, newUser.DisplayName, newUser.Email);

            await SearchUserData(newUser.UserId);

        });      
    }


    public async Task SearchUserData(string _uid)
    {
        var task = await FirebaseCRUD.SearchData("uid", _uid);

        if (task == null)
        {
            // 새로운 유저 데이터 생성
            await FirebaseCRUD.PushData(DataStore.userData);
            Debug.Log("새로운 유저 데이터 생성 완료");
        }
        else
        {
            // 기존 유저 데이터 가져오기
            DataStore.userData = FirebaseCRUD.GetData<UserData>(task);

            Debug.Log("기존 유저 데이터를 가져옵니다...");
            Debug.Log($"기존 유저 데이터: {DataStore.userData.uid}, {DataStore.userData.name}, {DataStore.userData.email}");
        }    
    }

    //유저 데이터 수정

    //유저 데이터 삭제
}
