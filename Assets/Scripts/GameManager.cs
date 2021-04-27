using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake() => Instance = this;

    public TMPro.TMP_Text BananaBalancelabel;

    // public List<float3> SpawnPointList = new List<float3>();
    public float SpawnRadius = 20f;

    public (int, int) TruckSpawnPoint = (24, 35);
    public Camera camera;

    int bananaBalance = 0;

    public int GetBananaBalance()
    {
        return bananaBalance;
    }
    public bool SetBananaBalance(int delta, bool check = false)
    {
        Debug.Log($"<color=green> {bananaBalance}   {delta}</color>");
        if (bananaBalance + delta < 0)
        {
            AudioManager.PlaySound("fail");
            BananaBalancelabel.transform.DOPunchScale(Vector3.up, 0.5f).OnComplete(() => { BananaBalancelabel.transform.localScale = Vector3.one; });
            var col = BananaBalancelabel.color;
            BananaBalancelabel.color = Color.red;
            BananaBalancelabel.DOColor(col, 0.3f);
            return false;
        }
        if (check) return true;
        bananaBalance += delta;
        if (delta > 0) AudioManager.PlaySound("money_2");
        else AudioManager.PlaySound("money_loss");

        if (bananaBalance < 0) bananaBalance = 0;
        BananaBalancelabel.text = $"{bananaBalance}";
        Vector3 end = BananaBalancelabel.transform.position;
        BananaBalancelabel.transform.DOPunchScale(Vector3.up, 0.5f).OnComplete(() => { BananaBalancelabel.transform.localScale = Vector3.one; });
        var c = BananaBalancelabel.color;
        BananaBalancelabel.color = Color.green;
        BananaBalancelabel.DOColor(c, 1f);
        return true;
    }

    [System.NonSerialized] public int TruckCount = 0;


    private void Start()
    {
        Time.timeScale = 1f;

        AudioManager.Instance.Init();
        camera = Camera.main;
        PopulateSpawnPoints();
        CameraControllerTopDown.Instance.InitCamera();
        MapController.Instance.InitMap();
        // MapController.Instance.TerrainMap = WorldGenerator.GenerateWorld(200, 50);
        // MapController.Instance.UpdateMap();
        SetBananaBalance(1000);
    }

    public void SpawnTruck()
    {
        var truckObject = Instantiate(DataContainer.Instance.TruckPrefab, gameObject.transform);
        truckObject.transform.position = MapUtils.GetWorldPosByCellWithLayer(TruckSpawnPoint);
    }
    void PopulateSpawnPoints()
    {
        // SpawnPointList = new List<float3>();
        // SpawnPointList.Add(new float3(50f, 50f, 0));
    }
    // EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    // private void LateUpdate()
    // {
    //     float2 target = new float2();
    //     var vecttarget = camera.ScreenToWorldPoint(Input.mousePosition);
    //     target.x = vecttarget.x;
    //     target.y = vecttarget.y;

    //     World.DefaultGameObjectInjectionWorld.GetExistingSystem<MonkeyMoveSystem>().target = target;
    // }
    public void Win()
    {
        AudioManager.PlaySound("win");
        winscreen.SetActive(true);
        var images = winscreen.GetComponentsInChildren<Image>();
        foreach (var item in images)
        {
            DOTween.To(() => item.color, x => item.color = x, Color.white, 0.5f);
        }
    }

    public void OnMusicButtonClicked()
    {
        AudioManager.ToggleMusic();
    }

    public void OnSfxButtonClicked()
    {
        AudioManager.ToggleSFX();
    }
    public GameObject confirmMenu;
    public void OnQuitButtonClicked()
    {
        Time.timeScale = 0;
        confirmMenu.SetActive(true);
    }
    public void OnQuitDeny()
    {
        Time.timeScale = 1;
        confirmMenu.SetActive(false);

    }
    public void OnQuitApprove()
    {
        Application.Quit();

    }

    public GameObject winscreen;
    public GameObject menuscreen;

    public void GoToGame()
    {
        var images = menuscreen.GetComponentsInChildren<Image>();
        foreach (var item in images)
        {
            DOTween.To(() => item.color, x => item.color = x, Color.clear, 0.5f);
        }
        Time.timeScale = 1f;
        Destroy(menuscreen, 1f);
    }

}
