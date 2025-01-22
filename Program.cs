using System;
using System.Diagnostics;
using Vosk;
using SFML.Graphics;
using SFML.Window;
using PortAudioSharp;

namespace Vac_cya.NET
{
    class Program
    {
        static float offset = 0.0f;

        static void Main(string[] args)
        {
            Vosk.Vosk.SetLogLevel(0);
            var model = new Model("src_/vosk-model-small-ru-0.22");
            var recognizer = new VoskRecognizer(model, 16000.0f);

            RenderWindow window = new RenderWindow(new VideoMode(800, 600), "Vac'cya.NET");
            window.SetFramerateLimit(25);

            var chatUser = new Text("Скажите что-нибудь", new Font("src_/ofont.ru_Times New Roman.ttf"));

            PortAudio.Initialize();

                var result = recognizer.Result();
                Console.WriteLine($"Вы сказали: {result}");
                chatUser.DisplayedString = ($"{result}");
                RunProgram(result);

                //PortAudio.Record();
                Console.WriteLine("Голосовой помощник запущен. Скажите что-нибудь...");

                window.Closed += (sender, e) => window.Close();
                window.KeyPressed += (sender, e) =>
                {
                    if (e.Code == Keyboard.Key.Escape)
                        window.Close();
                };

                // Основной цикл
                while (window.IsOpen)
                {
                    window.DispatchEvents();
                    window.Clear();

                    window.Draw(chatUser);
                    DrawWaves(window);

                    window.Display();
                }

                //waveIn.StopRecording();
            }
        

        private static void DrawWaves(RenderWindow window)
        {
            // Параметры для рисования волн
            var width = window.Size.X;
            var height = window.Size.Y;
            float amplitude = 50.0f;
            float frequency = 0.02f;

            // Создание линии для рисования
            VertexArray wave = new VertexArray(PrimitiveType.LineStrip, width);

            for (uint x = 0; x < width; x++)
            {
                float y = height / 2 + amplitude * (float)Math.Sin(frequency * (x + offset));
                wave[x] = new Vertex(new SFML.System.Vector2f(x, y), SFML.Graphics.Color.Cyan);
            }

            window.Draw(wave);

            offset += 2.0f;
        }

        private static void RunProgram(string command)
        {
            if (command.Contains("браузер"))
            {
                Process.Start("vivaldi");
                Speak("Открываю ваш любимый браузер");
            }
            else if (command.Contains("код"))
            {
                Process.Start("code");
                Speak("Покодим?");
            }
            else if (command.Contains("майн"))
            {
                Process.Start("java -jar LegacyLauncher_legacy.jar");
                Speak("Намёк понят");
            }
            else if (command.Contains("консоль") || command.Contains("терминал"))
            {
                Process.Start("zsh");
                Speak("Только не пиши sudo rm -rf /*");
            }
            else if (command.Contains("монтаж") || command.Contains("видеомонтаж"))
            {
                Process.Start("shotcut");
                Process.Start("capcut");

                Speak("Конечно, секунду");
            }
        }

        private static void Speak(string text)
        {
            // Используем Flite для TTS
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "flite",
                    Arguments = $"-t \"{text}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
} 