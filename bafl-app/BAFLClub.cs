using System;
namespace bafl_app
{
    public class BAFLClub
    {
        private string _region;
        private string _football;
        private string _cheer;
        private string _mascot;

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
        }

        public string Footbal
        {
            get => _football;
        }

        public string Cheer
        {
            get => _cheer;
        }

        public string Mascot
        {
            get => _mascot;
        }

        public override string ToString()
        {
            return String.Format("{} {}", Region, Footbal);
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

