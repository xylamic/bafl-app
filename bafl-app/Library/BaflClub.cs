using System;
namespace bafl_app.library
{
    public class BaflClub
    {
        private string _region;
        private string _football;
        private string _cheer;
        private string _mascot;
        private string _website;
        private string _president;
        private string _fieldName;
        private string _fieldLocation;

        public BaflClub() { }

        public BaflClub(string region, string football, string cheer,
            string mascot, string website, string fieldName, string fieldLocation)
        {
            _region = region;
            _football = football;
            _cheer = cheer;
            _mascot = mascot;
            _website = website;
            _fieldName = fieldName;
            _fieldLocation = fieldLocation;
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

        public string President
        {
            get => _president;
            set => _president = value;
        }

        public string FieldName
        {
            get => _fieldName;
            set => _fieldName = value;
        }

        public string FieldLocation
        {
            get => _fieldLocation;
            set => _fieldLocation = value;
        }

        public bool FieldPresent
        {
            get => !String.IsNullOrWhiteSpace(FieldLocation) && !String.IsNullOrWhiteSpace(FieldLocation);
        }

        public bool FieldNotPresent
        {
            get => !FieldPresent;
        }

        public bool WebsitePresent
        {
            get => !String.IsNullOrWhiteSpace(Website);
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

