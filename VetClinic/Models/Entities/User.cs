using System;

namespace VetClinic.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Theme { get; set; }
        public string Language { get; set; }

        public override bool Equals(object obj)
        {
            return obj is User user && Id == user.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

    public class Administrator
    {
        public User User { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is Administrator administrator)
                return User.Id == administrator.User.Id;
            else if(obj is Veterinarian veterinarian)
                return User.Id == veterinarian.User.Id;
            else return obj is User user  && User.Id == user.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(User.Id);
        }
    }

    public class Veterinarian
    {
        public User User { get; set; }
        public string Title { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Administrator administrator)
                return User.Id == administrator.User.Id;
            else if (obj is Veterinarian veterinarian)
                return User.Id == veterinarian.User.Id;
            else return obj is User user && User.Id == user.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(User.Id);
        }

        public override string ToString()
        {
            return User.Name + " " + User.Surname + ", " + Title;
        }
    }
}
