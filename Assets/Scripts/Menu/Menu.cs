using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private CameraController cameraController;
    public GameObject menuPanel;
    public Button quitBtn;

    public bool visibleMenu = false;

    // Start is called before the first frame update
    void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        menuPanel.SetActive(visibleMenu);

        quitBtn.onClick.AddListener(QuitBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            visibleMenu = !visibleMenu;

            menuPanel.SetActive(visibleMenu);
            cameraController.SetUIActiveCount(visibleMenu);
        }
    }

    private void QuitBtnClick()
    {
        // ���� ����
        Application.Quit();

        // ������ ��忡���� �������� �ʱ� ������, �����Ϳ����� ����Ǵ� ��ó�� ���̵��� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
