﻿using Assets._Project.Scripts.Cameras;
using Assets._Project.Scripts.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Assets._Project.Scripts.Cutscenes
{
    public class CutsceneManager : MonoBehaviour
    {
        public CharacterMaster Character;
        public Transform CharacterCamTarget;

        public Cutscene1Params CS1Params;

        private Dictionary<string, Func<IEnumerator>> _cutscenes;

        void Start()
        {
            _cutscenes = new Dictionary<string, Func<IEnumerator>>();
            _cutscenes.Add("cutscene_1", () => Cutscene1());
        }


        public static CutsceneManager Get()
        {
            return GameObject.Find("CutSceneManager").GetComponent<CutsceneManager>();
        }

        public void PlayCutscene(string name)
        {
            if (_cutscenes.ContainsKey(name))
            {
                StartCoroutine(PlayCutscene(_cutscenes[name]));
                return;
            }
            Debug.LogError("Tried to play cutscene " + name + ", but it doesn't exist");
        }

        private IEnumerator PlayCutscene(Func<IEnumerator> cutsceneCallback)
        {
            Character.SitDown();
            ShowCutsceneBorders();
            yield return cutsceneCallback();
            HideCutsceneBorders();
            Character.StopSittingDown();
        }

        private void HideCutsceneBorders()
        {
            // TODO:
        }

        private void ShowCutsceneBorders()
        {
            // TODO:
        }

        private IEnumerator Cutscene1()
        {
            yield return new WaitForEndOfFrame();
            Character.DisableControl();
            SetCameraTarget(CS1Params.Monolith.transform.GetChild(0)); // TODO: won't work later
            yield return new WaitForSeconds(2f);
            yield return new WaitForSeconds(SetPlayerCamera());
            Character.EnableControl();
        }

        private Vector3 _lastPlayerCamPosition;
        private Quaternion _lastPlayerCamRotation;

        private void SetCameraTarget(Transform transform)
        {
            var cam = Camera.main;
            cam.GetComponent<DepthOfField>().focalTransform = transform;
            cam.GetComponent<SmoothFollow>().enabled = false;
            cam.GetComponent<LookAt>().enabled = true;
            cam.GetComponent<LookAt>().Target = transform;
            _lastPlayerCamPosition = cam.transform.position;
            _lastPlayerCamRotation = cam.transform.rotation;
        }

        private float SetPlayerCamera()
        {
            var cam = Camera.main;
            float duration = 2f;
            cam.GetComponent<PositionLerp>().Lerp(cam.transform.position, _lastPlayerCamPosition, cam.transform.rotation, _lastPlayerCamRotation, duration);
            StartCoroutine(SetPlayerCameraAfter(duration));

            return duration;
        }

        private IEnumerator SetPlayerCameraAfter(float duration)
        {
            yield return new WaitForSeconds(duration);

            var cam = Camera.main;

            cam.GetComponent<DepthOfField>().focalTransform = CharacterCamTarget;
            cam.GetComponent<SmoothFollow>().target = CharacterCamTarget;
            cam.GetComponent<SmoothFollow>().enabled = true;
            cam.GetComponent<LookAt>().enabled = false;
        }
    }

    [Serializable]
    public class Cutscene1Params
    {
        public GameObject Monolith;
    }
}