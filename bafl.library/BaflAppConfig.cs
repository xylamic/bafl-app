using System;
namespace bafl.library
{
    public class BaflAppConfig
    {
        private string _key;

        public BaflAppConfig() { }

        public BaflAppConfig(string key)
        {
            _key = key;
        }

        public string Key
        {
            get => _key;
            set => _key = value;
        }
    }
}

