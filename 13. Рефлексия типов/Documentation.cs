using System;
using System.Linq;
using System.Reflection;

namespace Documentation
{
    // Класс Specifier реализует интерфейс ISpecifier и предоставляет методы для извлечения документации из атрибутов класса
    public class Specifier<T> : ISpecifier
    {
        // Храним тип класса для документации
        private readonly Type Type = typeof(T);

        // Метод для получения общего описания API, используя атрибут ApiDescriptionAttribute
        public string GetApiDescription() =>
            typeof(T).GetCustomAttribute<ApiDescriptionAttribute>()?.Description;

        // Метод для получения имен методов, помеченных атрибутом ApiMethodAttribute
        public string[] GetApiMethodNames() =>
            Type.GetMethods()
                .Where(x => x.GetCustomAttributes<ApiMethodAttribute>().Any()) // Фильтруем методы по атрибуту
                .Select(x => x.Name) // Извлекаем имена методов
                .ToArray();

        // Метод для получения описания конкретного метода по его имени
        public string GetApiMethodDescription(string methodName) =>
            Type.GetMethod(methodName)?
            .GetCustomAttribute<ApiDescriptionAttribute>()?
            .Description;

        // Метод для получения имен параметров указанного метода
        public string[] GetApiMethodParamNames(string methodName) =>
            Type.GetMethod(methodName)
            .GetParameters() // Получаем параметры метода
            .Select(x => x.Name) // Извлекаем имена параметров
            .ToArray();

        // Метод для получения описания конкретного параметра метода
        public string GetApiMethodParamDescription(string methodName, string paramName) =>
            Type.GetMethod(methodName)?
            .GetParameters() // Получаем параметры метода
            .Where(x => x.Name == paramName) // Фильтруем по имени параметра
            .FirstOrDefault()?
            .GetCustomAttribute<ApiDescriptionAttribute>()?
            .Description;

        // Метод для получения полного описания параметра метода
        public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string paramName)
        {
            var param = Type.GetMethod(methodName)?
                .GetParameters()?
                .Where(x => x.Name == paramName)
                .FirstOrDefault();

            // Если параметр не найден, возвращаем описание по умолчанию
            if (param is null)
                return new ApiParamDescription()
                {
                    ParamDescription =
                    new CommonDescription(paramName == string.Empty ? null : paramName)
                };

            // Возвращаем полное описание параметра с учетом атрибутов
            return new ApiParamDescription
            {
                ParamDescription = new CommonDescription(
                paramName, param.GetCustomAttribute<ApiDescriptionAttribute>()?.Description),
                MinValue = param.GetCustomAttribute<ApiIntValidationAttribute>()?.MinValue,
                MaxValue = param.GetCustomAttribute<ApiIntValidationAttribute>()?.MaxValue,
                Required = param.GetCustomAttribute<ApiRequiredAttribute>() is null ?
                false : param.GetCustomAttribute<ApiRequiredAttribute>().Required
            };
        }

        // Метод для получения полного описания метода, включая описание параметров и возвращаемого значения
        public ApiMethodDescription GetApiMethodFullDescription(string methodName)
        {
            var method = Type.GetMethod(methodName);
            // Проверяем, есть ли атрибут ApiMethodAttribute у метода
            if (method.GetCustomAttribute<ApiMethodAttribute>() is null)
                return null;

            // Создаем объект описания метода
            var description = new ApiMethodDescription
            {
                ParamDescriptions = GetApiMethodParamNames(methodName)
                .Select(param => GetApiMethodParamFullDescription(methodName, param)) // Получаем полное описание параметров
                .ToArray(),
                MethodDescription = new CommonDescription(methodName, GetApiMethodDescription(methodName)),
                ReturnDescription = GetApiMethodReturnParamFullDescription(methodName) // Получаем описание возвращаемого значения
            };

            return description; // Возвращаем полное описание метода
        }

        // Метод для получения полного описания возвращаемого параметра метода
        private ApiParamDescription GetApiMethodReturnParamFullDescription(string methodName)
        {
            var param = Type.GetMethod(methodName)?.ReturnParameter; // Получаем возвращаемый параметр метода
            // Если метод возвращает void, возвращаем null
            if (param.ParameterType.Name == "Void")
                return null;

            // Возвращаем полное описание возвращаемого параметра
            return new ApiParamDescription()
            {
                ParamDescription = new CommonDescription(
                null, param.GetCustomAttribute<ApiDescriptionAttribute>()?.Description),
                MinValue = param.GetCustomAttribute<ApiIntValidationAttribute>()?.MinValue,
                MaxValue = param.GetCustomAttribute<ApiIntValidationAttribute>()?.MaxValue,
                Required = param.GetCustomAttribute<ApiRequiredAttribute>() is null ?
                false : param.GetCustomAttribute<ApiRequiredAttribute>().Required
            };
        }
    }
}
