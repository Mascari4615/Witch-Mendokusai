using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine.UI;

public class GoogleLoginTest : MonoBehaviour
{
    public TextMeshProUGUI GoogleStatusText;

    void Start()
    {
        // The following grants profile access to the Google Play Games SDK.
        // Note: If you also want to capture the player's Google email, be sure to add
        // .RequestEmail() to the PlayGamesClientConfiguration
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //    .AddOauthScope("profile")
        //    .RequestServerAuthCode(false)
        //    .Build();
        //PlayGamesPlatform.InitializeInstance(config);

        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;

        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    void Update()
    {
    }

    public void OnSignInButtonClicked()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                GoogleStatusText.text = "Google Signed In";
                // var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                // Debug.Log("Server Auth Code: " + serverAuthCode);
                Debug.Log("Server Auth Code: ");

                PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                {
                    TitleId = PlayFabSettings.TitleId,
                    // ServerAuthCode = serverAuthCode,
                    CreateAccount = true
                }, (result) => { GoogleStatusText.text = "Signed In as " + result.PlayFabId; }, OnPlayFabError);
            }
            else
            {
                GoogleStatusText.text = "Google Failed to Authorize your login";
            }
        });
    }

    private void OnPlayFabError(PlayFabError obj)
    {
        GoogleStatusText.text = "Playfab error";
    }
}