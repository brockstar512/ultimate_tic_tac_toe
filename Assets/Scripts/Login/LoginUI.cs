using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LoginUI : MonoBehaviour
{
    CanvasGroup current;
    [SerializeField] CanvasGroup splashPage;
    [SerializeField] CanvasGroup forgotPW;
    [SerializeField] CanvasGroup signUp;
    [SerializeField] CanvasGroup login;
    [SerializeField] Button forgotPWButton;
    [SerializeField] Button signUpButton;
    [SerializeField] Button loginButton;
    [SerializeField] Button landingSignUpButton;
    [SerializeField] Button landingLoginButton;

    private void Awake()
    {
        current = splashPage;
        signUpButton.onClick.AddListener(delegate { Open(signUp); });
        landingSignUpButton.onClick.AddListener(delegate { Open(signUp); } );
        loginButton.onClick.AddListener(delegate { Open(login); } );
        landingLoginButton.onClick.AddListener(delegate { Open(login); });

    }

    void Open(CanvasGroup cg)
    {
        current.blocksRaycasts = false;
        current.DOFade(0, .1f).OnComplete(() => cg.DOFade(1, .5f).OnComplete(() => current = cg));
        current.blocksRaycasts = true;

    }

}
