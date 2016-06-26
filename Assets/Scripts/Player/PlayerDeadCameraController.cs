﻿using UnityEngine;
using System.Collections;

public class PlayerDeadCameraController : MonoBehaviour
{
    [SerializeField] float m_deathCameraPanSpeed = 20f;
    [SerializeField] ParticleSystem m_explosionParticles;
    [SerializeField] ParticleSystem m_waterSplashParticles;
    [SerializeField] ParticleSystem m_fireParticles;

    private bool m_dead;


    void Update()
    {
        if (m_dead)
            transform.Rotate(Vector3.up, m_deathCameraPanSpeed * Time.deltaTime, Space.World);
    }


    private void PlayerDead(string colliderTag)
    {
        m_dead = true;
        transform.rotation = Quaternion.identity;

        if (colliderTag == Tags.Water && m_waterSplashParticles != null)
            Instantiate(m_waterSplashParticles, transform.position, Quaternion.identity);

        if (m_explosionParticles != null)
            Instantiate(m_explosionParticles, transform.position, Quaternion.identity);

        if (m_fireParticles != null)
            Instantiate(m_fireParticles, transform.position, Quaternion.identity);

        var cameraPosition = transform.GetChild(0);

        Camera.main.transform.parent = cameraPosition;
        Camera.main.transform.position = cameraPosition.position;
        Camera.main.transform.rotation = cameraPosition.rotation;

        transform.parent = null;

        EventManager.TriggerEvent(BooleanEventName.ActivateHud, false);
    }


    void OnEnable()
    {
        EventManager.StartListening(StringEventName.PlayerDead, PlayerDead);
    }


    void OnDisable()
    {
        EventManager.StopListening(StringEventName.PlayerDead, PlayerDead);
    }
}
