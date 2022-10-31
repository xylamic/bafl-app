using System;
namespace bafl.library
{
    public class BAFLClub
    {
        private string _region;
        private string _football;
        private string _cheer;
        private string _mascot;

        public BAFLClub() { }

        public BAFLClub(string region, string football, string cheer,
            string mascot)
        {
            _region = region;
            _football = football;
            _cheer = cheer;
            _mascot = mascot;
        }

        public string Region
        {
            get => _region;
            set => _region = value;
        }

        public string Football
        {
            get => _football;
            set => _football = value;
        }

        public string Cheer
        {
            get => _cheer;
            set => _cheer = value;
        }

        public string Mascot
        {
            get => _mascot;
            set => _mascot = value;
        }

        public override string ToString()
        {
            return String.Format("{} {}", Region, Football);
        }

        public string FootballName()
        {
            return this.ToString();
        }

        public string CheerName()
        {
            return String.Format("{} {}", Region, Cheer);
        }

        public string MascotName()
        {
            return String.Format("{} {}", Region, Mascot);
        }
    }
}

