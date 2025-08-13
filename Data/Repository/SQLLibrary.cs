using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Lorecraft_API.Data.DTO.Account;
using Lorecraft_API.Data.DTO.Character;
using Lorecraft_API.Data.DTO.Concept;
using Lorecraft_API.Data.DTO.Context;
using Lorecraft_API.Data.DTO.Country;
using Lorecraft_API.Data.DTO.Description;
using Lorecraft_API.Data.DTO.Faction;
using Lorecraft_API.Data.DTO.Lore;
using Lorecraft_API.Data.DTO.Story;
using Lorecraft_API.Data.Models;
using static Lorecraft_API.Resources.Constants;

namespace Lorecraft_API.Data.Repository
{
    internal static partial class SQLWriter
    {
        private const string DISABLE_TRACKING = "SET TRANSACTION READ ONLY";
        private const string SINGLE_QUOTATION_MARK = @"""";
        private const string INSERT_START = @"INSERT INTO public";
        private const string UPDATE_START = @"UPDATE public";
        private const string SELECT_START = @"SELECT";
        private const string DELETE_START = @"DELETE";
        private const string GROUP_BY = "GROUP BY";
        private const string ORDER_BY = "ORDER BY";
        private const string HAVING = "HAVING";
        private const string VALUES = "VALUES";
        private const string JOIN = "JOIN";
        private const string LEFT_JOIN = "LEFT JOIN";
        private const string ON = "ON";
        private const string UNDER_SCORE = "_";
        private const string ASCENDING = "ASC";
        private const string DESCENDING = "DESC";
        private const char DOT = '.';
        private const char SPACE = ' ';

        #region DataProperties

        private static readonly string[] AccountProperties =
        [
         nameof(Account.AccountId),
         nameof(Account.PfpId),
         nameof(Account.FirstName),
         nameof(Account.MiddleName),
         nameof(Account.LastName),
         nameof(Account.Birthdate),
         nameof(Account.Gender),
         nameof(Account.IsEmailVerified),
         nameof(Account.Email),
         nameof(Account.ContactNum),
         nameof(Account.IsContactVerified),
         nameof(Account.IsSpammer),
         nameof(Account.Password),
         nameof(Account.Role),
         nameof(Account.TempPassword),
         nameof(Account.PenName),
         nameof(Account.CountryCodeContact),
         nameof(Account.DatetimeCreated)
        ];

        private static readonly string[] CharacterProperties =
[
            nameof(Character.CharacterId),
            nameof(Character.LoreId),
            nameof(Character.FactionId),
            nameof(Character.CountryId),
            nameof(Character.PersonId),
            nameof(Character.Personality),
            nameof(Character.Background),
            nameof(Character.Purpose),
            nameof(Character.Passions),
            nameof(Character.Nickname)
        ];

        private static readonly string[] ConceptProperties
        =
        [
            nameof(Concept.ConceptId),
            nameof(Concept.LoreId),
            nameof(Concept.Name),
            nameof(Concept.Type)
        ];

        private static readonly string[] ContextProperties
        =
        [
            nameof(Context.CharacterId),
            nameof(Context.ContextId),
            nameof(Context.FactionId),
            nameof(Context.CountryId),
            nameof(Context.ContextValue),
            nameof(Context.Date)
        ];

        private static readonly string[] DescriptionProperties =
        [
            nameof(Description.DescriptionId),
            nameof(Description.ConceptId),
            nameof(Description.Type),
            nameof(Description.Content),
            nameof(Description.OrderN)
        ];

        private static readonly string[] FactionProperties =
        [
            nameof(Faction.Name),
            nameof(Faction.Type),
            nameof(Faction.FactionId),
            nameof(Faction.LoreId),
            nameof(Faction.Purpose)
        ];

        private static readonly string[] LoreProperties
        =
        [
            nameof(Lore.UniverseName),
            nameof(Lore.AccountId),
            nameof(Lore.LoreId),
            nameof(Lore.IsPublic)
        ];

        private static readonly string[] PersonProperties =
        [
         nameof(Person.PersonId),
         nameof(Person.Birthdate),
         nameof(Person.FirstName),
         nameof(Person.MiddleName),
         nameof(Person.LastName),
         nameof(Person.Birthdate),
         nameof(Person.Nationality)
        ];

        private static readonly string[] StoryProperties =

        [
         nameof(Story.StoryId),
         nameof(Story.LoreId),
         nameof(Story.Title),
         nameof(Story.Description),
         nameof(Story.GenreId),
         nameof(Story.DocLink)
        ];

        private static readonly string[] CountryProperties =
        [
            nameof(Country.CountryId),
            nameof(Country.Continent),
            nameof(Country.GovernmentType),
            nameof(Country.Name),
            nameof(Country.LoreId)
        ];





        #endregion

        #region GeneralFunction

        internal static string AsNoTracking(this string sql)
        => string.Join("; ", DISABLE_TRACKING, sql + ';');

        internal static string GenerateUpdateSqlForDefaultSchemaWithoutEntity<TRequest>(TRequest request, string entityName)
        where TRequest : class
        {
            string product = string.Join(DOT, UPDATE_START, FormatEntityName(entityName));

            product += " SET ";

            ValidateThenIncludeProperties(ref product, request, ConceptProperties, out bool result);

            return result ? product : string.Empty;

        }

        internal static string GenerateUpdateSqlForDefaultSchema<TEntity, TRequest>(TEntity entity, TRequest request)
        where TEntity : class
        where TRequest : class
        {

            var entityIdentity = IdentifyEntity(entity);

            string entityName = string.Intern(entityIdentity.Item1);

            string[] props = entityIdentity.Item2;

            string product = string.Join(DOT, UPDATE_START, FormatEntityName(entityName));

            product += " SET ";

            if (props.Length == 0)
                return string.Empty;

            // Console.WriteLine($"Sending props: {props}");
            // PrintArrays(props);

            ValidateThenIncludeProperties(ref product, request, props, out bool result);

            return result ? product : string.Empty;

        }

        internal static string GenerateInsertSqlForDefaultScheme<TEntity>(TEntity entity, string[] pascalProperties)
        where TEntity : class
        {

            var entityIdentity = IdentifyEntity(entity);

            string entityName = string.Intern(entityIdentity.Item1);

            string[] snakeCasedProps = entityIdentity.Item2;

            string product = string.Join(DOT, INSERT_START, FormatEntityName(entityName));

            if (snakeCasedProps.Length == 0)
                return string.Empty;

            CraftInsertParameters(ref product, snakeCasedProps);

            return product;

        }

        internal static string GenerateSelectSqlWithIdentityForDefaultSchema(string idName)
        {
            string entityName = IdentifyEntity(idName);

            return GenerateSelectSqlForDefaultSchema(entityName, idName);
        }

        internal static string GenerateSelectSqlWithIdentityAndJoinForDefaultSchema(string entityName, string[] idNames)
        {
            string secondEntityName = IdentifyEntity(idNames[1]);

            return GenerateSelectJoinSqlForDefaultSchema(entityName, secondEntityName, idNames);
        }


        internal static string GenerateSelectSqlWithSpecificPropsForDefaultSchema(string entityName, string[] props)
        {
            string[] snakeCasedProps = SnakeCasingProperties(props);

            string product = string.Intern(SELECT_START);

            CraftReadingProperties(ref product, entityName, string.Empty, snakeCasedProps);

            return product;

        }

        internal static string GenerateSelectSqlWithSpecificParametersForDefaultSchema(string entityName, string[] parameters)
        {
            string[] props = GetDataProperties(entityName);
            string[] snakeCasedProps = SnakeCasingProperties(props);

            string product = string.Intern(SELECT_START);

            CraftReadingProperties(ref product, entityName, parameters, snakeCasedProps);

            return product;
        }
        internal static string GenerateSelectSqlWithSortByForDefaultSchema(string entityName, string[] parameters, string[] groupBys, string? havingProp, string[] orderBys, SortingMode mode)
        {
            string[] props = GetDataProperties(entityName);
            string[] snakeCasedProps = SnakeCasingProperties(props);

            string product = string.Intern(SELECT_START);

            CraftReadingProperties(ref product, entityName, parameters, snakeCasedProps);
            AddSort(ref product, groupBys, havingProp, orderBys, entityName.ToLower()[0], mode);

            return product;
        }


        internal static string GenerateSelectSqlForDefaultSchema(string entityName, string? idName)
        {
            string[] props = GetDataProperties(entityName);
            string[] snakeCasedProps = SnakeCasingProperties(props);

            string product = string.Intern(SELECT_START);

            CraftReadingProperties(ref product, entityName, idName, snakeCasedProps);

            return product;

        }

        internal static string GenerateSelectJoinSqlForDefaultSchema(string entityName, string secondEntityName, string[] idNames)
        {
            string[] firstProps = SnakeCasingProperties(GetDataProperties(entityName));
            string[] secondProps = SnakeCasingProperties(GetDataProperties(secondEntityName));

            string product = string.Intern(SELECT_START);

            CraftReadingPropertiesWithJoin(ref product, [entityName, secondEntityName], idNames, firstProps, secondProps);

            return product;

        }

        internal static string GenerateDeleteSqlForDefaultSchema(string idName)
        {
            string entityName = IdentifyEntity(idName);

            string product = string.Intern(DELETE_START);

            CraftDeleteStatement(ref product, entityName, idName);

            return product;
        }

        internal static string GenerateDeleteSqlWithMultipleIdentitiesForDefaultSchema(string idName, string[] ids)
        {
            string entityName = IdentifyEntity(idName);

            string product = string.Intern(DELETE_START);

            CraftDeleteStatementMultiple(ref product, entityName, idName, ids);

            return product;
        }


        internal static Func<Character, Person, Character> CharacterAndPerson = (character, person) =>
        {
            character.Person = person;
            return character;
        };

        internal static Func<Concept, Description[], Concept> ConceptAndDescriptions = (concept, descriptions) =>
        {
            concept.Descriptions = descriptions;
            return concept;
        };

        #endregion

        #region PrivateFunctions

        // private static void PrintArrays(string[] pr)
        // {
        //     foreach (string p in pr)
        //     {
        //         Console.WriteLine(p);
        //     }
        // }

        private static void CraftDeleteStatement(ref string product, string entityName, string idName)
        {
            product += string.Join(SPACE, SPACE + "FROM", $"public.{FormatEntityName(entityName)}" + SPACE);

            string snakeCasedId = ToSnakeCase(idName);

            product += string.Join(SPACE, SPACE + "WHERE", $"{snakeCasedId} = @{idName}");

        }

        private static void CraftDeleteStatementMultiple(ref string product, string entityName, string idName, string[] ids)
        {
            product += string.Join(SPACE, SPACE + "FROM", $"public.{FormatEntityName(entityName)}" + SPACE);

            string snakeCasedId = ToSnakeCase(idName);

            product += string.Join(SPACE, SPACE + "WHERE", $"{snakeCasedId} IN (");

            for (int i = 0; i < ids.Length; i++)
            {
                product += ids[i] + ", ";
            }

            if (product.EndsWith(", "))
                product = product[..^2];

            product += ')';
        }

        private static void CraftInsertParameters(ref string product, string[] snakeProperties)
        {
            CraftFormulas(ref product, snakeProperties, out Dictionary<int, string> propertyLocations);
            CraftValueParameters(ref product, propertyLocations);
        }

        private static void CraftFormulas(ref string product, string[] snakeCasedProperties, out Dictionary<int, string> propertyLocations)
        {
            propertyLocations = [];
            product += "(";
            for (int i = 0; i < snakeCasedProperties.Length; i++)
            {
                string prop = string.Intern(snakeCasedProperties[i]);
                product += i != snakeCasedProperties.Length - 1 ? string.Join(',', prop, " ") : prop;
                propertyLocations.Add(i, ToFormalCase(prop));
            }
            product += ")";
        }

        private static void CraftReadingProperties(ref string product, string entityName, string? idName, string[] snakeCasedProperties)
        {
            if (snakeCasedProperties.Length <= 0)
                return;

            string snakeCasedId = string.Empty;
            for (int i = 0; i < snakeCasedProperties.Length; i++)
            {
                string prop = string.Intern(snakeCasedProperties[i]);

                if (string.IsNullOrEmpty(prop))
                    break;

                if (!string.IsNullOrEmpty(idName) && prop.Replace(string.Intern(UNDER_SCORE), string.Empty).Equals(idName, StringComparison.OrdinalIgnoreCase))
                    snakeCasedId = string.Intern(prop);

                product += $" {prop} {ToFormalCase(prop)}, ";
            }

            if (product.EndsWith(", "))
                product = product[..^2] + SPACE;

            string additionalWhereClause = !string.IsNullOrEmpty(snakeCasedId) ? string.Join(" = ", snakeCasedId, $"@{ToFormalCase(snakeCasedId)}") : string.Empty;
            product += string.Join(DOT, "FROM public", FormatEntityName(entityName));
            product += !string.IsNullOrEmpty(additionalWhereClause) ? string.Join(' ', " WHERE", string.Intern(additionalWhereClause)) : string.Empty;
        }
        private static void CraftReadingPropertiesWithJoin(ref string product, string[] entityNames, string[] idNames, string[] firstProps, string[] secondProps)
        {
            if (firstProps.Length <= 0 || secondProps.Length <= 0)
                return;

            string firstSnakeCasedId = string.Empty;
            string secondSnakeCasedId = firstSnakeCasedId;
            char firstTableLetter = entityNames[0].ToLower()[0];
            char secondTableLetter = entityNames[1].ToLower()[0];
            string firstProduct = string.Empty;
            string secondProduct = string.Empty;
            ConcurrentBag<string> firstProductParts = [];
            ConcurrentBag<string> secondProductParts = [];

            void firstProductProcess()
            {
                foreach (string prop in firstProps)
                {
                    firstProductParts.Add($"{string.Join(DOT, firstTableLetter, prop)} {ToFormalCase(prop)}");

                    if (!string.IsNullOrEmpty(idNames[0]) && prop.Replace(string.Intern(UNDER_SCORE), string.Empty).Equals(idNames[0], StringComparison.OrdinalIgnoreCase))
                        firstSnakeCasedId = string.Intern(idNames[0]);
                }
            }

            void secondProductProcess()
            {
                foreach (string sProp in secondProps)
                {

                    secondProductParts.Add($"{string.Join(DOT, secondTableLetter, sProp)} {ToFormalCase(sProp)}");

                    if (!string.IsNullOrEmpty(idNames[1]) && sProp.Replace(string.Intern(UNDER_SCORE), string.Empty).Equals(idNames[1], StringComparison.OrdinalIgnoreCase))
                        secondSnakeCasedId = string.Intern(idNames[1]);
                }
            }

            Parallel.Invoke(firstProductProcess, secondProductProcess);

            CheckAndFinalizeProductParts(ref firstProduct, firstProductParts);
            CheckAndFinalizeProductParts(ref secondProduct, secondProductParts);

            product += SPACE + string.Join(", ", firstProduct, secondProduct) + SPACE;

            string additionalWhereClause = !string.IsNullOrEmpty(firstSnakeCasedId) ? string.Join(" = ", ToSnakeCase(firstSnakeCasedId), $"@{firstSnakeCasedId}") : string.Empty;
            string additionalJoinClause = !string.IsNullOrEmpty(secondSnakeCasedId) ? string.Join(" = ", string.Join(DOT, firstTableLetter, ToSnakeCase(secondSnakeCasedId)), string.Join(DOT, secondTableLetter, ToSnakeCase(secondSnakeCasedId))) : string.Empty;
            product += string.Join(DOT, "FROM public", FormatEntityName(entityNames[0])) + SPACE + firstTableLetter;
            product += !string.IsNullOrEmpty(additionalJoinClause) ? string.Join(SPACE, SPACE + LEFT_JOIN, $"public.{FormatEntityName(entityNames[1])} {secondTableLetter}", ON, string.Intern(additionalJoinClause)) : string.Empty;
            product += !string.IsNullOrEmpty(additionalWhereClause) ? string.Join(SPACE, " WHERE", string.Intern(additionalWhereClause)) : string.Empty;
        }

        private static void CheckAndFinalizeProductParts(ref string product, ConcurrentBag<string> parts)
        {
            if (parts.IsEmpty)
                return;

            List<string> formattedParts = [.. parts.Select(p => p.Trim())];
            product = string.Join(", ", formattedParts);

            if (product.EndsWith(", "))
            {
                product = product[..^2];
            }
        }


        private static void CraftReadingProperties(ref string product, string entityName, string[] parameters, string[] snakeCasedProperties)
        {
            if (snakeCasedProperties.Length <= 0)
                return;

            int count = 0;
            string whereClause = string.Empty;
            char tableLetter = entityName.ToLower()[0];

            for (int i = 0; i < snakeCasedProperties.Length; i++)
            {
                string prop = string.Intern(snakeCasedProperties[i]);

                if (string.IsNullOrEmpty(prop))
                    break;

                product += $" {string.Join(DOT, tableLetter, prop)} {ToFormalCase(prop)}, ";

                if (count < parameters.Length)
                {
                    if (!string.IsNullOrEmpty(parameters[count]) && prop.Replace(string.Intern(UNDER_SCORE), string.Empty).Equals(parameters[count], StringComparison.OrdinalIgnoreCase))
                    {
                        whereClause += count == 0 ? $" {string.Join(DOT, tableLetter, prop)} = @{parameters[count]} " : $" AND {string.Join(DOT, tableLetter, prop)} = @{parameters[count]} ";
                        count++;
                    }
                }
            }

            if (product.EndsWith(", "))
                product = product[..^2] + SPACE;

            product += string.Join(DOT, "FROM public", string.Join(SPACE, FormatEntityName(entityName), tableLetter));
            product += !string.IsNullOrEmpty(whereClause) ? string.Join(' ', " WHERE", string.Intern(whereClause)) : string.Empty;
        }
        private static void AddSort(ref string product, string[] groupBys, string? havingProp, string[] orderBys, char tableLetter, SortingMode mode)
        {
            product += SPACE + GROUP_BY + SPACE;

            for (int i = 0; i < groupBys.Length; i++)
            {
                product += groupBys[i] + ", ";
            }

            if (product.EndsWith(", "))
                product = product[..^2];

            product += SPACE;

            if (!string.IsNullOrEmpty(havingProp))
                product += HAVING + SPACE + string.Join(DOT, tableLetter, havingProp);

            if (orderBys.Length != 0)
            {
                product += SPACE + ORDER_BY + SPACE;

                for (int j = 0; j < orderBys.Length; j++)
                {
                    product += string.Join(DOT, tableLetter, ToSnakeCase(orderBys[j])) + ", ";
                }
            }

            if (product.EndsWith(", "))
                    product = product[..^2];

            product += SPACE;

            product += mode == SortingMode.Descending ? DESCENDING : ASCENDING;

        }

        private static void CraftValueParameters(ref string product, Dictionary<int, string> propertyLocations)
        {
            product += $" {VALUES}(";
            for (int i = 0; i < propertyLocations.Count; i++)
            {
                if (!propertyLocations.TryGetValue(i, out string? prop))
                    return;

                if (string.IsNullOrEmpty(prop))
                    return;

                product += $"@{prop}, ";
            }

            if (product.EndsWith(", "))
                product = product[..^2];

            product += ")";
        }

        private static void ValidateThenIncludeProperties<TRequest>(
      ref string product, TRequest request, string[] properties, out bool result)
     where TRequest : class
        {
            string[] flexParams;
            result = false;

            if (request is not (LoreRequest or AccountUpdateRequest or ChangePasswordRequest or CharacterUpdateRequest or PersonUpdateRequest
            or ConceptRequest or StoryUpdateRequest or ContextUpdateRequest or FactionUpdateRequest or DescriptionUpdateRequest or CountryUpdateRequest))
                return;

            flexParams = request switch
            {
                AccountUpdateRequest accRequest => accRequest.GetParams(),
                ChangePasswordRequest => [nameof(ChangePasswordRequest.AccountId), nameof(ChangePasswordRequest.NewPassword)],
                CharacterUpdateRequest charRequest => charRequest.GetParams(),
                PersonUpdateRequest personRequest => personRequest.GetParams(),
                LoreRequest loreRequest => loreRequest.GetParams(),
                ConceptRequest conceptRequest => conceptRequest.GetParams(),
                StoryUpdateRequest storyUpdateRequest => storyUpdateRequest.GetParams(),
                ContextUpdateRequest contextUpdateRequest => contextUpdateRequest.GetParams(),
                FactionUpdateRequest factionUpdateRequest => factionUpdateRequest.GetParams(),
                DescriptionUpdateRequest descriptionUpdateRequest => descriptionUpdateRequest.GetParams(),
                CountryUpdateRequest countryUpdateRequest => countryUpdateRequest.GetParams(),

                _ => []
            };

            // Console.WriteLine($"Sending Params: {flexParams}");
            // PrintArrays(flexParams);

            if (flexParams.Length > 0)
            {
                IncludeProperties(ref product, request, properties, flexParams);
                result = true;
            }
        }


        private static void IncludeProperties<TRequest>(
            ref string product, TRequest request, string[] properties, string[] parameters)
            where TRequest : class
        {
            string idProp = IdentifyID(request);
            string? snakeCasedIDProp = properties.FirstOrDefault(p => p.Replace(string.Intern(UNDER_SCORE), string.Empty).Equals(idProp, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(snakeCasedIDProp))
                return;

            List<string> setClauses = [];

            foreach (string prop in properties)
            {
                if (prop.Replace(string.Intern(UNDER_SCORE), string.Empty).Equals(idProp, StringComparison.OrdinalIgnoreCase))
                    continue;

                string paramMapping = SearchParam(prop, ref parameters);
                if (!string.IsNullOrEmpty(paramMapping))
                {
                    setClauses.Add(paramMapping);
                }
            }

            if (setClauses.Any())
            {
                product += string.Join(", ", setClauses);
                product += $" WHERE {snakeCasedIDProp} = @{idProp}";
            }
        }


        private static string SearchParam(string prop, ref string[] parameters)
        {
            foreach (string param in parameters)
            {
                if (prop.Replace(string.Intern(UNDER_SCORE), string.Empty).Equals(param, StringComparison.OrdinalIgnoreCase)
                || IsPasswordNecessaryToChange(prop, param))
                {
                    return $"{prop} = @{param}";
                }
            }
            return string.Empty;
        }

        private static string FormatEntityName(string entityName) => SINGLE_QUOTATION_MARK + entityName + SINGLE_QUOTATION_MARK;

        private static (string, string[]) IdentifyEntity<TEntity>(TEntity entity)
    where TEntity : class => entity is Account or Person or Character or Lore or Concept or
                Context or Description or Faction or Story or Country ? ProcessEntity(entity) : (string.Empty, Array.Empty<string>());


        private static string IdentifyEntity(string idName)
        => idName switch
        {
            nameof(Account.AccountId) or nameof(Account.Email) or nameof(Account.PenName) => nameof(Account),
            nameof(Lore.LoreId) => nameof(Lore),
            nameof(Person.PersonId) => nameof(Person),
            nameof(Character.CharacterId) => nameof(Character),
            nameof(Story.StoryId) => nameof(Story),
            nameof(Concept.ConceptId) => nameof(Concept),
            nameof(Context.ContextId) => nameof(Context),
            nameof(Faction.FactionId) => nameof(Faction),
            nameof(Description.DescriptionId) => nameof(Description),
            nameof(Country.CountryId) => nameof(Country),

            _ => None
        };

        private static string[] GetDataProperties(string entityName)
        => entityName switch
        {
            nameof(Account) => AccountProperties,
            nameof(Character) => CharacterProperties,
            nameof(Concept) => ConceptProperties,
            nameof(Context) => ContextProperties,
            nameof(Description) => DescriptionProperties,
            nameof(Faction) => FactionProperties,
            nameof(Lore) => LoreProperties,
            nameof(Person) => PersonProperties,
            nameof(Story) => StoryProperties,
            nameof(Country) => CountryProperties,

            _ => []
        };

        private static (string, string[]) ProcessEntity<TEntity>(TEntity entity)
     where TEntity : class
        {
            if (entity is null)
                return (string.Empty, Array.Empty<string>());

            return entity switch
            {
                Account acc => (nameof(Account), SnakeCasingProperties(acc.Properties)),
                Lore lore => (nameof(Lore), SnakeCasingProperties(lore.Properties)),
                Character character => (nameof(Character), SnakeCasingProperties(character.Properties)),
                Person person => (nameof(Person), SnakeCasingProperties(person.Properties)),
                Story story => (nameof(Story), SnakeCasingProperties(story.Properties)),
                Context context => (nameof(Context), SnakeCasingProperties(context.Properties)),
                Faction faction => (nameof(Faction), SnakeCasingProperties(faction.Properties)),
                Country country => (nameof(Country), SnakeCasingProperties(country.Properties)),
                Concept concept => (nameof(Concept), SnakeCasingProperties(concept.Properties)),
                Description description => (nameof(Description), SnakeCasingProperties(description.Properties)),


                _ => (None, Array.Empty<string>())
            };

        }

        private static string[] SnakeCasingProperties(string[] props) => [.. props.Select(ToSnakeCase)];

        private static string ToSnakeCase(string prop) => !string.IsNullOrEmpty(prop) ? SnakeCaseRegex().Replace(prop, "$1_$2").ToLower()
        : string.Empty;

        private static string ToFormalCase(string snake) => !string.IsNullOrEmpty(snake) ? string.Join("", snake.Split('_').Select(word => char.ToUpper(word[0]) + word[1..]))
        : string.Empty;


        private static string IdentifyID<TRequest>(TRequest request)
        where TRequest : class
         => request switch
         {
             AccountUpdateRequest or ChangePasswordRequest => nameof(AccountUpdateRequest.AccountId),
             LoreRequest => nameof(LoreRequest.LoreId),
             CharacterUpdateRequest => nameof(CharacterUpdateRequest.CharacterId),
             PersonUpdateRequest => nameof(PersonUpdateRequest.PersonId),
             ContextUpdateRequest => nameof(ContextUpdateRequest.ContextId),
             ConceptRequest => nameof(ConceptRequest.ConceptId),
             StoryUpdateRequest => nameof(StoryUpdateRequest.StoryId),
             FactionUpdateRequest => nameof(FactionUpdateRequest.FactionId),
             CountryUpdateRequest => nameof(CountryUpdateRequest.CountryId),

             _ => string.Empty

         };

        private static bool IsPasswordNecessaryToChange(string prop, string param) => prop.Equals("password") && PasswordPropRegex().Match(param).Success;


        // static string ProcessingSnakeCase(string pro)
        // {
        //     string product = pro;
        //     string quotient = string.Empty;
        //     bool isFirstTime = true;

        //     for (int i = 1; i < product.Length; i++)
        //     {
        //         if (char.IsUpper(product[i]))
        //         {
        //             if (isFirstTime)
        //             {
        //                 quotient = string.Join('_', product[..i].ToLower(), product[i..].ToLower());
        //                 isFirstTime = false;
        //                 continue;
        //             }
        //             string baseSecond = product[i..].ToLower();
        //             quotient = string.Join('_', quotient.Replace(baseSecond, string.Empty), baseSecond);
        //         }
        //     }

        //     return quotient;


        #endregion


        [GeneratedRegex("([a-z0-9])([A-Z])")]
        private static partial Regex SnakeCaseRegex();
        [GeneratedRegex(@"(\w)+Password")]
        private static partial Regex PasswordPropRegex();

    }
}