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
            _logs.Info("Initializing speech synthesizer");

            try
            {
                _speechSynthesizer = new SpeechSynthesizer();
            }
            catch (Exception ex)
            {
                _logs.Error(ex, "Error creating speech synthesizer");
            }

            //Now we need to get voice that supports RU
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ru-ru");
            var voicesList = _speechSynthesizer.GetInstalledVoices(ci);
            if (voicesList.Count < 1)
            {
                _logs.Error(new Exception(), "No support for Russian language installed");
            }
            _speechSynthesizer.SelectVoice(voicesList[0].VoiceInfo.Name);
        }

        public void Text2File(in string text, out string filepath)
        {
            filepath = Path.GetTempFileName();

            try
            {
                _speechSynthesizer.SetOutputToWaveFile(filepath);
                _speechSynthesizer.Speak(text);
                _speechSynthesizer.SetOutputToNull();
            }
            catch (Exception ex)
            {
                _logs.Error(ex, "Error converting text to wave file");
            }
        }
    }
}
