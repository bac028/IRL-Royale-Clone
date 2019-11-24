﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class SignUp : MonoBehaviour
{
    private PlayerData data;
    public InputField emailInput, passwordInput, confirmPasswordInput, username;

    private DatabaseReference databaseReference;

    private string DATA_URL = "https://iroyale-1571440677136.firebaseio.com/";

    //Update panel

    public GameObject comfPanel;

    public GameObject ErrorPanel;

    public GameObject errorText;

    private bool is_SignUp = false;

    private bool is_errorMessage = false;

    private string msg = "";

    // Start is called before the first frame update
    void Start() {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(DATA_URL);
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateNewUser() {
        if (emailInput.text.Equals("") || passwordInput.text.Equals("") || confirmPasswordInput.text.Equals("")
            || username.text.Equals(""))
        {
            Debug.Log("all fields not filled in");
            OpenPanel("all fields not filled in");

            return;
        }

        if (!passwordInput.text.Equals(confirmPasswordInput.text))
        {
            Debug.Log("password and confirm not the same");
            OpenPanel("password and confirm not the same");
            return;
        }

        Register(emailInput.text, passwordInput.text, username.text);
    }

    public void Register(string emailInput, string passwordInput, string username)
    {
        if (emailInput.Equals("") && passwordInput.Equals("") && username.Equals(""))
        {
            return;
        }

        FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(emailInput, 
            passwordInput).ContinueWith((task =>
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
            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

            //Update
            data = new PlayerData(this.username.text, FirebaseAuth.DefaultInstance.CurrentUser.UserId);

            string jsonData = JsonUtility.ToJson(data);
            if (FirebaseAuth.DefaultInstance.CurrentUser != null && FirebaseAuth.DefaultInstance.CurrentUser.Email != "")
            {
                print("user id: " + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
                databaseReference.Child("users").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).
                  SetRawJsonValueAsync(jsonData);
                is_SignUp = true;
            }
        }));

    }

    void GetErrorMessage(AuthError errorCode)
    {
        msg = "";
        msg = errorCode.ToString();
        is_errorMessage = true;
        OpenPanel(msg);
        Debug.Log(msg);
    }

    public void OpenPanel()
    {
        if(comfPanel != null)
        {
            comfPanel.SetActive(true);
        }
    }

    public void OpenPanel(string msg)
    {
        if(comfPanel != null)
        {
            bool isActive = ErrorPanel.activeSelf;
            ErrorPanel.SetActive(!isActive);
            errorText.SetActive(true);
            errorText.GetComponent<Text>().text = msg;

        }
        if (is_errorMessage)
        {
            is_errorMessage = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (is_SignUp)
        {
            OpenPanel();
        }
        if (is_errorMessage)
        {
            OpenPanel(msg);
        }
    }
}