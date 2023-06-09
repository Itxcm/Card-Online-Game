﻿using UnityEngine;
using System.Collections;

namespace Crosstales.RTVoice.Tool
{
    /// <summary>Simple sequencer for dialogues.</summary>
    //[ExecuteInEditMode]
    [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_sequencer.html")]
    public class Sequencer : MonoBehaviour
    {
        #region Variables

        /// <summary>All available sequences.</summary>
        [Tooltip("All available sequences.")]
        public Model.Sequence[] Sequences;

        /// <summary>Fallback culture for all sequences (e.g. 'en', optional).</summary>
        [Tooltip("Fallback culture for all sequences (e.g. 'en', optional).")]
        public string Culture;

        /// <summary>Delay in seconds before the Sequencer starts processing (default: 0).</summary>
        [Tooltip("Delay in seconds before the Sequencer starts processing (default: 0).")]
        public float Delay = 0f;

        /// <summary>Run the Sequencer on start on/off (default: off).</summary>
        [Tooltip("Run the Sequencer on start on/off (default: off).")]
        public bool PlayOnStart = false;

        private int currentIndex = 0;

        private System.Guid currentSpeaker;

        private bool playAllSequences = false;

        #endregion


        #region Properties

        /// <summary>Returns the current Sequence.</summary>
        public Model.Sequence CurrentSequence
        {
            get { return Sequences[currentIndex]; }
        }

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (PlayOnStart)
            {
                PlayAllSequences();
            }

            // Subscribe event listeners
            Speaker.OnSpeakComplete += speakCompleteMethod;
        }

        public void OnDestroy()
        {
            // Unsubscribe event listeners
            Speaker.OnSpeakComplete -= speakCompleteMethod;
        }

        public void OnValidate()
        {
            foreach (Model.Sequence seq in Sequences)
            {
                if (!seq.initalized)
                {
                    seq.Rate = 1f;
                    seq.Pitch = 1f;
                    seq.Volume = 1f;

                    seq.initalized = true;
                }
            }
        }

        #endregion


        #region Public methods
        /// <summary>Plays a Sequence with a given index.</summary>
        /// <param name="index">Index of the Sequence (default: 0, optional).</param>
        public void PlaySequence(int index = 0)
        {
            if (Sequences != null)
            {
                if (index >= 0 && index < Sequences.Length)
                {
                    StartCoroutine(playMe(Sequences[index]));

                    currentIndex = index + 1;
                }
                else
                {
                    Debug.LogWarning("The given index is outside the range of Sequences: " + index);
                }
            }
            else
            {
                Debug.LogWarning("Sequences is null!");
            }
        }

        /// <summary>Plays the next Sequence in the array.</summary>
        public void PlayNextSequence()
        {
            PlaySequence(currentIndex);
        }

        /// <summary>Plays all Sequences.</summary>
        public void PlayAllSequences()
        {
            StopAllSequences();

            playAllSequences = true;

            PlaySequence();
        }

        /// <summary>Stops and silences all active Sequences.</summary>
        public void StopAllSequences()
        {
            StopAllCoroutines();
            Speaker.Silence();
            playAllSequences = false;
        }

        #endregion


        #region Callback methods

        private void speakCompleteMethod(Model.Event.SpeakEventArgs e)
        {
            if (playAllSequences)
            {
                if (e.Wrapper.Uid.Equals(currentSpeaker) && currentIndex < Sequences.Length)
                {
                    PlayNextSequence();
                }
                else
                {
                    StopAllSequences();
                }
            }
        }

        #endregion


        #region Private methods

        private IEnumerator playMe(Model.Sequence seq)
        {
            yield return new WaitForSeconds(Delay);

            Model.Voice voice = Speaker.VoiceForName(seq.RTVoiceName);

            if (voice == null)
            {
                voice = Speaker.VoiceForCulture(Culture);
            }

            if (seq.Mode == Model.SpeakMode.Speak)
            {
                currentSpeaker = Speaker.Speak(seq.Text, seq.Source, voice, true, seq.Rate, seq.Volume, "", seq.Pitch);
            }
            else
            {
                currentSpeaker = Speaker.SpeakNative(seq.Text, voice, seq.Rate, seq.Volume, seq.Pitch);
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)