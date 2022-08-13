using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MasterChief_ExplosionForce : MonoBehaviour
{
    public float radius = 10;
    public float Force = 5;

    public Color gizmoscolor = Color.yellow;

    public string layerName = "Default";

    public void AddForce()
    {
        try
        {
            List<Collider> listHitCollider = new List<Collider>();

            if (layerName.NotNullOrEmpty())//ͨ��layerName��ȡ
            {
                listHitCollider = Physics.OverlapSphere(transform.position, radius, 1 << LayerMask.NameToLayer(layerName)).ToList();
            }
            else//ͨ���ض��ű�ExplosionForceTarget��ȡ
            {
                var tempColliders = Physics.OverlapSphere(transform.position, radius);
                foreach (var collider in tempColliders)
                {
                    if (collider.GetComponentInParent<MasterChief_ExplosionForceTarget>())
                    {
                        listHitCollider.AddOnce(collider);
                    }
                }
            }

            //Get All Rigidbody
            List<Rigidbody> listRig = new List<Rigidbody>();
            foreach (var collider in listHitCollider)
            {
                Rigidbody rig = collider.attachedRigidbody;
                if (rig)
                {
                    listRig.AddOnce(rig);
                }
            }

            //Add Force
            foreach (var rig in listRig)
            {
                rig.AddForce(-(transform.localPosition - rig.transform.localPosition) * Force, ForceMode.Impulse);
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = gizmoscolor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
