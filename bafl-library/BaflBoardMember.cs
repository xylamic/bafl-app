using System;
using System.Data;

namespace bafl.library
{
    public class BaflBoardMember
    {
        private string _role;
        private string _name;
        private string _email;

        public BaflBoardMember() { }

        public BaflBoardMember(string role, string name, string email)
        {
            _role = role;
            _name = name;
            _email = email;
        }

        public string Role
        {
            get => _role;
            set => _role = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Email
        {
            get => _email;
            set => _email = value;
        }

        public bool EmailPresent
        {
            get => !String.IsNullOrEmpty(Email);
        }

        public override string ToString()
        {
            return String.Format("{}: {}", Role, Name);
        }
    }
}

