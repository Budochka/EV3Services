using System;
using Microsoft.Speech.Recognition;

namespace VoiceRecognition
{
    class Program
    {
        static bool done = false;

        static void Main(string[] args)
        {
            // Select a speech recognizer that supports English.  
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            {
                Console.WriteLine(ri.Culture);
            }

            try
            {
                Console.WriteLine("I'm listening");

                System.Globalization.CultureInfo ci =
                  new System.Globalization.CultureInfo("ru-ru");
                SpeechRecognitionEngine sre =
                  new SpeechRecognitionEngine(ci);
                sre.SetInputToDefaultAudioDevice();
                sre.SpeechRecognized += sre_SpeechRecognized;
                sre.RecognizeCompleted += sre_RecognizeCompleted;

                Choices colorChoices = new Choices();
                colorChoices.Add("направо");
                colorChoices.Add("налево");
                colorChoices.Add("вперёд");
                colorChoices.Add("назад");
                colorChoices.Add("всё"); // quit

                GrammarBuilder colorsGrammarBuilder =
                  new GrammarBuilder();
                colorsGrammarBuilder.Culture = ci;
                colorsGrammarBuilder.Append(colorChoices);
                Grammar keyWordsGrammar =
                  new Grammar(colorsGrammarBuilder);
                sre.LoadGrammarAsync(keyWordsGrammar);

                sre.RecognizeAsync(RecognizeMode.Multiple);

                while (done == false) {; }

                Console.WriteLine("Done");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        } // Main

        static void sre_SpeechRecognized(object sender,
          SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "всё")
            {
                ((SpeechRecognitionEngine)sender).RecognizeAsyncCancel();
                return;
            }
            if (e.Result.Confidence >= 0.75)
                Console.WriteLine("I heard " + e.Result.Text);
            else
                Console.WriteLine("Unknown word");
        }

        static void sre_RecognizeCompleted(object sender,
          RecognizeCompletedEventArgs e)
        {
            done = true;
        }
    } // class Program
} // ns