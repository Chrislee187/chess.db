using System;
using System.Linq;

namespace chess.games.db.api.Services
{
    public class PersonName
    {
        public string Firstname { get; }
        public string Middlename { get; }
        public string Lastname { get; }

        public PersonName(string firstname, string middlename, string lastname)
        {
            Firstname = firstname;
            Middlename = middlename;
            Lastname = lastname;
        }

        public static bool TryParse(string text, out PersonName personName)
        {
            string firstname = null, middleName = null, lastname;

            // Names with more than three parts cannot be analysed
            if (text.Split(' ').Length > 3)
            {
                personName = null;
                return false;
            }

            try
            {
                if (text.Contains(","))
                {
                    (firstname, middleName, lastname) = ParseReversedName(text);
                }
                else if (text.Contains(' '))
                {
                    (firstname, middleName, lastname) = ParseName(text);
                }
                else
                {
                    lastname = text;
                }

                personName = new PersonName(firstname, middleName, lastname);
            }
            catch
            {
                personName = null;
                return false;
            }

            return true;
        }

        private static (string firstname, string middlename, string lastname) ParseName(string text)
        {
            string firstname = null, middleName = null, lastname;
            var names = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            switch (names.Length)
            {
                case 1:
                    lastname = names[0];
                    break;
                case 2:
                    firstname = names[0];
                    lastname = names[1];
                    break;
                default:
                    firstname = names[0];
                    lastname = names[^1];
                    middleName = JoinNames(names.AsSpan(1, names.Length-2));
                    break;
            }

            firstname = firstname?.TrimEnd('.');
            lastname = lastname?.TrimEnd('.');
            return (firstname, middleName, lastname);
        }
        private static (string firstname, string middlename, string lastname) ParseReversedName(string text)
        {
            string middleName = null;
            var firstNameLast = text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToArray();

            if (firstNameLast.Length > 2) throw new ArgumentException($"Too many ({firstNameLast.Length}) comma's found in name '{text}'", nameof(text));

            var firstname = firstNameLast[1].TrimEnd('.');

            var otherNames = SplitNames(firstNameLast[0]);

            var lastname = otherNames.Last();

            if (otherNames.Length >= 2)
            {
                middleName = JoinNames(otherNames.AsSpan(0,otherNames.Length - 1));
            }
            else if (firstname.Contains(' '))
            {
                var firstNames = SplitNames(firstname);
                firstname = firstNames.First();
                middleName = JoinNames(firstNames.AsSpan(1));
            }

            return (firstname, middleName, lastname);
        }

        private static string JoinNames(Span<string> names)
            => string.Join(' ', names.ToArray());
        private static string[] SplitNames(string text)
            => text.Split(' ').Select(t => t.Trim()).ToArray();
    }
}