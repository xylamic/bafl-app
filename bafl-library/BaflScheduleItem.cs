using System;
namespace bafl.library
{
    public class BaflClub
    {
        private string _region;
        private string _football;
        private string _cheer;
        private string _mascot;
        private string _website;

        public BaflClub() { }

        public BaflClub(string region, string football, string cheer,
            string mascot, string website)
        {
            _region = region;
            _football = football;
            _cheer = cheer;
            _mascot = mascot;
            _website = website;
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

        public string RegionFootball
        {
            get => String.Format("{0} {1}", _region, _football);
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

        public string Website
        {
            get => _website;
            set => _website = value;
        }

        public bool WebsitePresent
        {
            get => !String.IsNullOrEmpty(Website);
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

