using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Threeyes.EventPlayer;
using Threeyes.GameFramework;
using Threeyes.Core;

public class MasterChief_Grenade : GenerableObject<MasterChief_Grenade.ConfigInfo>
{
    //Self
    public Rigidbody rig;
    public DelayEventPlayer delayEventPlayer;
    public MasterChief_ExplosionForce masterChief_ExplosionForce;

    public GameObject preExplodeParticle;

    public override void Init(ConfigInfo config)
    {
        base.Init(config);

        delayEventPlayer.DelayTime = config.explodeDelayTime;
        delayEventPlayer.Play();

        //Todo：设置masterChief_ExplosionForce的范围
    }


    public void OnDelayEventPlayerPlay()
    {
        //Explode
        masterChief_ExplosionForce.AddForce();
        GameObject goEffect = preExplodeParticle.InstantiatePrefab(null, transform.position, transform.rotation);
        goEffect.transform.localScale = transform.lossyScale;//同步缩放
        Destroy(gameObject);
    }

    #region Testing
    public float testVelocityScale = 1;
    public void AddVelocity(Vector3 velocity)
    {
        rig.AddForce(velocity * testVelocityScale, ForceMode.Impulse);
    }
    #endregion

    #region Define
    [System.Serializable]
    [JsonObject(MemberSerialization.Fields)]
    public class ConfigInfo
    {
        [Range(1, 10)] public float explodeDelayTime = 3f;//ToUse
    }
    #endregion
}
//测试