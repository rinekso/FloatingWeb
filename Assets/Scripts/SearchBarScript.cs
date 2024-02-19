using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TLab.Android.WebView;

public class SearchBarScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField m_searchBar;
    [SerializeField]
    TLabWebView m_webview;
    public void SearchGoogle()
    {
        var https = "https://";
        var http = "http://";
        var hedder = "";
        if (m_searchBar.text.Length > http.Length)
        {
            if (m_searchBar.text.Substring(0, https.Length - 1) != https &&
                m_searchBar.text.Substring(0, http.Length - 1) != http)
            {
                hedder = "https://www.google.com/search?q=";
            }
        }
        else
        {
            hedder = "https://www.google.com/search?q=";
        }

        m_webview.LoadUrl(hedder + m_searchBar.text);
    }
}
