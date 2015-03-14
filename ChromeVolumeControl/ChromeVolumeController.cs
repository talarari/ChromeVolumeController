using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CSCore.CoreAudioAPI;

namespace ChromeVolumeControl
{
    public class ChromeVolumeController
    {
         private readonly Predicate<Process> _predicate;

         public ChromeVolumeController(Predicate<Process> predicate)
        {
            _predicate = predicate;
        }

        public void SetVolume(int volume)
        {
            var volumes = Observable.Create<SimpleAudioVolume>(observer =>
            {
                var timer = new System.Timers.Timer { Interval = 100 };
                timer.Elapsed += (sender, args) =>
                {
                    var audioVolumes = GetDefaultAudioSessionManager2(DataFlow.Render)
                       .GetSessionEnumerator()
                       .Select(session => session.QueryInterface<AudioSessionControl2>())
                       .Where(session2 => _predicate(session2.Process))
                       .Select(relevantSession => relevantSession.QueryInterface<SimpleAudioVolume>())
                       .ToList();

                    if (audioVolumes.Count > 0)
                    {
                        audioVolumes.ForEach(observer.OnNext);
                        observer.OnCompleted();

                        if (timer != null)
                        {
                            timer.Stop();
                            timer.Dispose();
                        }

                    }


                };
                timer.Start();
                return Disposable.Empty;
            });


            volumes.Subscribe(audioVolume => audioVolume.MasterVolume = volume / 100f);

        }

        private AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }

    }
}
