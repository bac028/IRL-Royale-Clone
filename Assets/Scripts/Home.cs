﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;

public class Home : MonoBehaviour
{
    private DatabaseReference databaseReference;

    private bool signed_in = false;

    private string DATA_URL = "https://iroyale-1571440677136.firebaseio.com/";

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATA_URL);
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        if(signed_in)
            SceneManager.LoadScene("MapBox");
    }

    public void GoToLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void GoToSignup()
    {
        SceneManager.LoadScene("Signup");
    }

    public void GoToHome()
    {
        SceneManager.LoadScene("Home");
    }

    public void LogInAnonymous()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith((task =>
        {
            if (task.IsCanceled)
            {
                Firebase.FirebaseException e =
              task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);
                return;
            }
            if (task.IsFaulted)
            {

                Firebase.FirebaseException e =
                task.Exception.Flatten().InnerExceptions[0] as Firebase.FirebaseException;

                GetErrorMessage((AuthError)e.ErrorCode);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            LoginInfo.IsGuest = true;
            LoginInfo.Uid = newUser.UserId;

            signed_in = true;
        }));
    }

    void GetErrorMessage(AuthError errorCode)
    {
        string msg = "";
        msg = errorCode.ToString();
        Debug.Log(msg);
    }
}