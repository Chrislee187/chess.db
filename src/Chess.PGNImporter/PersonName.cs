namespace Chess.PGNImporter;

public class PersonName
{
    public string? FirstName { get; }
    public string? MiddleName { get; }
    public string LastName { get; }

    public PersonName(string? firstName, string? middleName, string lastName)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
    }

    public static bool TryParse(string text, out PersonName? personName)
    {
        if (text.Split(' ').Length > 3)
        {
            personName = null;
            return false;
        }

        try
        {
            string? firstname = null;
            string? middleName = null;
            string lastname;
            if (text.Contains(','))
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
        string? firstname = null;
        string? middleName = null;
        string? lastname;
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
                middleName = JoinNames(names.AsSpan(1, names.Length - 2));
                break;
        }

        firstname = firstname?.TrimEnd('.') ?? "";
        lastname = lastname.TrimEnd('.'); 
        middleName = middleName?.TrimEnd('.') ?? "";
        return (firstname, middleName, lastname);
    }
    private static (string firstname, string middlename, string lastname) ParseReversedName(string text)
    {
        var middleName = "";
        var firstNameLast = text.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .ToArray();

        if (firstNameLast.Length > 2) throw new ArgumentException($"Too many ({firstNameLast.Length}) comma's found in name '{text}'", nameof(text));

        var firstname = firstNameLast[1].TrimEnd('.');

        var otherNames = SplitNames(firstNameLast[0]);

        var lastname = otherNames.Last();

        if (otherNames.Length >= 2)
        {
            middleName = JoinNames(otherNames.AsSpan(0, otherNames.Length - 1));
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