using System;
using System.Collections.Generic;
using System.Linq;
using amulware.Graphics;
using Cireon.Audio;
using yatl.Environment;
using yatl.Utilities;

/* 
 * SHOULD HAVE
 * Musicparameters should change smoothly
 * Make transition to dark faster
 * Make tension go up when reaching win
 * Wav files
 * Use higher octaves when light, either hardcoded or procedural
 * 
 * COULD HAVE
 * More instrument samples
 * Several instruments for single note
 * Broken chords
 * Don't randomly walk through graph, but consider form also
 * Automatic asyncopes
 * "Opbouw" when staying in a subgraph
 * 
 * NOTES
 * Why not pass MusicParameters to Update method?
 * Implementing looping of buffers for ASR sounds is not reasonably doable
 * 
 * */

namespace yatl
{
    sealed class MusicManager
    {
        double time = 0;

        LinkedList<SoundEvent> eventSchedule = new LinkedList<SoundEvent>();
        BranchingMusicalComposition composition;
        Motif currentMotif;
        public bool Winning = false;

        public static Random Random = new Random();
        public static List<Sound> SustainSet = new List<Sound>();
        public static double Speed = 1;
        public static double MaxSpeed = 1;
        public static double MinSpeed = 1;
        public static double Acceleration = 1;
        public static double Volume = 1;
        public static double OutOfTune = 0;

        public Instrument Piano;
        public Instrument Violin;
        public Instrument Strings;
        OggStream ambient;

        public MusicParameters Parameters { get; set; }

        public MusicManager()
        {
            AudioManager.Initialize();
            this.ambient = new OggStream("data/music/ambient1.ogg");
            //this.Piano = new SimpleInstrument("data/music/Piano.pp.C4_2.ogg", 261.6);
            this.Piano = new Piano();
            this.Violin = new SRInstrument("data/music/ViolinGis3-loop.ogg", "data/music/ViolinGis3-decay.ogg", 207.7);
            //this.Strings = new SimpleInstrument("data/music/StringsC5.ogg", 523.3);
            this.Strings = new SRInstrument("data/music/StringsC5-sustain.ogg", "data/music/StringsC5-decay.ogg", 523.3);

            string filename = "data/music/DarkAndLight.bmc";
            Console.WriteLine("Parsing " + filename);
            this.composition = new BranchingMusicalComposition(filename);

            this.ambient.IsLooped = true;
            this.ambient.Prepare();

            this.Parameters = new MusicParameters(1f, 0f, 1f, GameState.GameOverState.Undetermined);
        }

        public void Schedule(IEnumerable<SoundEvent> soundEvents)
        {
            soundEvents = soundEvents.OrderBy(o => o.StartTime);

            // Place the soundevents after the last event that is currently scheduled
            double offset;
            if (this.eventSchedule.Count == 0)
                offset = this.time;
            else
                offset = this.eventSchedule.Last.Value.StartTime;

            foreach (var soundEvent in soundEvents) {
                soundEvent.AddOffset(offset);
                this.eventSchedule.AddLast(soundEvent);
            }
        }

        void scheduleNextMotif()
        {
            if (this.currentMotif == null)
                this.currentMotif = this.composition.Motifs["light1a"];
            else {
                if (this.Parameters.GameOverState == GameState.GameOverState.Won) {
                    var win = this.currentMotif.Successors.Where(o => o.Name.Contains("win")).ToList();
                    if (win.Count() != 0)
                        this.currentMotif = win[0];
                }
                else if (this.Parameters.GameOverState == GameState.GameOverState.Lost) {
                    var lose = this.currentMotif.Successors.Where(o => o.Name.Contains("lose")).ToList();
                    if (lose.Count() != 0)
                        this.currentMotif = lose[0];
                }

                string tag = this.Parameters.Lightness > .5 ? "light" : "dark";
                var choiceSpace = this.currentMotif.Successors.Where(o => o.Name.Contains(tag));
                if (choiceSpace.Count() == 0)
                    choiceSpace = this.currentMotif.Successors;
                this.currentMotif = choiceSpace.RandomElement();
            }

            RenderParameters parameters = new RenderParameters(this.Parameters, this.Piano);
            this.Schedule(this.currentMotif.Render(parameters));
        }

        public void Update(UpdateEventArgs args)
        {
            Speed = Math.Min(MaxSpeed, Speed + args.ElapsedTimeInS * Acceleration);
            double elapsedTime = args.ElapsedTimeInS * Speed;
            this.time += elapsedTime;

            double lightness = this.Parameters.Lightness;
            double tension = 0;
            if (this.currentMotif != null && this.currentMotif.Name.Contains("dark"))
                tension = this.Parameters.Tension;
            if (this.Parameters.GameOverState == GameState.GameOverState.Won)
                tension = 1;

            //OutOfTune = tension * 5;
            //OutOfTune = (1 - this.Parameters.Health) * 5;
            OutOfTune *= Math.Pow(0.3, elapsedTime);

            MaxSpeed = 1 + 0.5 * tension;
            MinSpeed = 0.4;// +0.2 * tension;
            Volume = 0.5 + tension;
            this.ambient.Volume = (float)(0.25 * (tension + 1 - lightness));

            //if (this.ambient.Ready)
                //this.ambient.Play();

            // Play soundevents
            while (this.eventSchedule.Count != 0 && this.eventSchedule.First.Value.StartTime <= this.time) {
                var nextEvent = this.eventSchedule.First.Value;
                this.eventSchedule.RemoveFirst();
                //Console.WriteLine(nextEvent);
                nextEvent.Execute();
            }

            // Schedule soundevents
            if (this.currentMotif == null || (this.currentMotif.Name != "win" && this.currentMotif.Name != "lose")) {
                if (this.eventSchedule.Count == 0)
                    this.scheduleNextMotif();
                else {
                    // If current motif ends in less than 1 seconds, schedule next motif
                    if (this.eventSchedule.Last.Value.StartTime - 1 < this.time)
                        this.scheduleNextMotif();
                }
            }

            AudioManager.Instance.Update((float)elapsedTime);
        }

        private static readonly object outOfTuneEventListLock = new object();
        private static List<Func<bool>> outOfTuneEvents = new List<Func<bool>>(); 

        public static void AddOutOfTuneEvent(Func<bool> @event)
        {
            lock (outOfTuneEventListLock)
            {
                outOfTuneEvents.Add(@event);
            }
        }

        public static bool TryPlayOutOfTune()
        {
            List<Func<bool>> events;
            lock (outOfTuneEventListLock)
            {
                if (outOfTuneEvents.Count == 0)
                    return false;
                events = outOfTuneEvents;
                outOfTuneEvents = new List<Func<bool>>();
            }
            return events.Aggregate(false, (current, e) => current || e());
        }
    }
}
