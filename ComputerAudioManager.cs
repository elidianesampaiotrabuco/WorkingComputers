using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace WorkingComputers
{
    public class ComputerAudioManager : MonoBehaviour
    {
        PropagatedAudioManager subPropAudio = new PropagatedAudioManager();
        RealComputerComponent computer;

        void Start()
        {
            //initialize
            subPropAudio.audioDevice = gameObject.AddComponent<AudioSource>();
            computer = gameObject.GetComponent<RealComputerComponent>();
        }
    }
}
