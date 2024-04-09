using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    static internal class ErrorMessages
    {
        static public string EmptyError { get; private set; } = "Поле не должно быть пустым";

        static public string LessCharacterError { get; private set; } = "Пароль должен содержать 8 символов";

        static public string SpacePasswordError { get; private set; } = "Пароль не должен содержать пробелы";

        static public string MatchError { get; private set; } = "Пароли должны совпадать";

        static public string RegisterError { get; private set; } = "Такой аккаунт уже существует";

    }
}
