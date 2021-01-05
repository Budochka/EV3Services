using System;
using System.IO;
using Microsoft.Speech.Synthesis;
using NLog;

namespace VoiceCreator
{
    class VoiceSynthesis
    {
        private Logger _logs;
        private SpeechSynthesizer _speechSynthesizer;

        public VoiceSynthesis(Logger log)
        { 
            _logs = log;
            _logs.Debug("Initializing speech synthesizer");

            try
            {
                _speechSynthesizer = new SpeechSynthesizer();
            }
            catch (Exception ex)
            {
                _logs.Error(ex, "Error creating speech synthesizer");
            }
        }

        public bool Text2File(in string text, out string filepath)
        {
            filepath = GetTempFileName();

            try
            {
                _speechSynthesizer.SetOutputToWaveFile(filepath);
            }
            catch (Exception ex)
            {
                _logs.Error(ex, "Error converting text to wave file");
            }
        }
    }
}
