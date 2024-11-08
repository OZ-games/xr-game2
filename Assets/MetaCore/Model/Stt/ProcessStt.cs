using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pnc.Model;
using Pnc.Model.Stt;

namespace util
{
    public class ProcessStt : MonoBehaviour
    {
        private static ProcessStt _instance = null; //singleton object

        [System.Serializable]
        public class StrEvent : UnityEvent<string>
        {
        }

        [Header("Events")]
        public StrEvent ParseStt = new StrEvent();

        Pnc.AndroidServiceBridge asb;
        // Start is called before the first frame update
        void Start()
        {
            if (_instance==null)
            {
                _instance = this;
            } else {
                Destroy(this.gameObject);
                return;
            }
            asb = this.GetComponent<Pnc.AndroidServiceBridge>();
            asb.eventStt.AddListener(OnMsgSttUpdated);
        }

        public static ProcessStt Instance() {
            if(!Exists()) {
                throw new PncException("[ProcessStt] could not find the ProcessStt object.");
            }

            return _instance;
        }

        public static bool Exists()
        {
            return _instance != null;
        }
        
        void OnMsgSttUpdated(string jsonParam)
        {
            EventSttResult evt;
            try
            {
                evt = PncJson<EventSttResult>.deserialize(jsonParam);
                string _resultDText = evt.getSttResult().getResultText();
                //Debug.Log($"ProcessStt OnMsgSttUpdated vvk _resultDText:{_resultDText}");
                ParseStt.Invoke(_resultDText);
            }
            catch (PncException e)
            {
                Debug.LogError($":::::     [ERROR] json param has been occurred Exception. what={e.Message}     :::::");
            }
        }

        private void OnDestroy()
        {
            asb.eventStt.RemoveAllListeners();
        }
    }
}