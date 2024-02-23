using System.Collections;
using System.Collections.Generic;
using Threeyes.Core;
using Threeyes.Coroutine;
using UnityEngine;

public class RocketShip_Controller : MonoBehaviour
    , IAC_CursorState_ChangedHandler
{
    public Transform tfMeteorRoot;
    public Renderer rendererWindow;
    public List<GameObject> listMeteorPrefab = new List<GameObject>();

    //Config
    public bool canGenerateMeteor = true;
    public float meteorLifeTime = 60;
    public Vector2 velocityPowerRange = new Vector2(3, 5);
    public Vector2 meteorSizeRange = new Vector3(0.5f, 1f);
    public Vector2 boredGenerateIntervalRange = new Vector2(2, 10);

    //Runtime
    List<GameObject> listGoMeteor = new List<GameObject>();//Runtime generated meteor

    #region Callback
    public void OnCursorStateChanged(AC_CursorStateInfo cursorStateInfo)
    {
        //��ǰStateΪ����ʱ����ʱ������ʯȺ
        bool isHiding = IsHidingState(cursorStateInfo.cursorState);
        tfMeteorRoot.gameObject.SetActive(!isHiding);


        //ToAdd:Boredʱ���������ʯ
        if (cursorStateInfo.cursorState == AC_CursorState.Bored)
        {
            if (cursorStateInfo.stateChange == AC_CursorStateInfo.StateChange.Enter)
            {
                TryStopCoroutine();
                cacheEnum = CoroutineManager.StartCoroutineEx(IEBoredRandomGenerateMeteor());
            }
            else
            {
                TryStopCoroutine();
            }
        }
    }
    protected UnityEngine.Coroutine cacheEnum;
    protected virtual void TryStopCoroutine()
    {
        if (cacheEnum != null)
            CoroutineManager.StopCoroutineEx(cacheEnum);
    }
    IEnumerator IEBoredRandomGenerateMeteor()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(boredGenerateIntervalRange.x, boredGenerateIntervalRange.y));
            GenerateMeteor(true);
        }
    }

    #endregion

    #region Invoke by Extern
    public void SetCanGenerateMeteor(bool value)
    {
        canGenerateMeteor = value;
    }
    public void GenerateMeteor(bool isDown)
    {
        if (!canGenerateMeteor)
            return;
        if (!isDown)
            return;
        if (IsHidingState(AC_ManagerHolder.StateManager.CurCursorState))
            return;

        StartCoroutine(IEGenerateMeteor(meteorLifeTime));
    }

    IEnumerator IEGenerateMeteor(float lifeTime)
    {
        ///Todo:��������ӷ�Χ�⣨����������󷽵�ƽ�棩���������ʯ��Ȼ�������׶��Χ��ĳ��λ�÷��䡣����Ӻ�����
        if (listMeteorPrefab.Count == 0)
            yield break;

        GameObject goMeteroInst = Instantiate(listMeteorPrefab.Random(new System.Random()));
        RocketShip_MeteorController meteorController = goMeteroInst.GetComponent<RocketShip_MeteorController>();
        goMeteroInst.transform.SetParent(tfMeteorRoot);
        meteorController.Init(lifeTime, meteorSizeRange, velocityPowerRange);
    }
    public void ChangeWindowTexture(Texture texture)
    {
        rendererWindow.material.mainTexture = texture;
    }
    #endregion

    static bool IsHidingState(AC_CursorState cursorState)//��ToUpdate����Ϊͨ�÷�����
    {
        return cursorState == AC_CursorState.Exit || cursorState == AC_CursorState.Hide || cursorState == AC_CursorState.StandBy;
    }
}
