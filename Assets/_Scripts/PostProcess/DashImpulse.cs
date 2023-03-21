using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
/// 
namespace Myd.Platform {
    public class DashImpulse : MonoBehaviour
    {
        private CinemachineImpulseSource source;

        void Awake()
        {
            source = this.GetComponent<CinemachineImpulseSource>();
        }

        public void Shake(Vector2 dir)
        {
            source.m_DefaultVelocity = dir;
            source.GenerateImpulseWithForce(0.1f);
        }
    }
}