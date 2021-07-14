using Humanizer;
using PimApi.ConsoleApp.Queries;
using PimApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PimApi.ConsoleApp
{
    // Moved to partial class to reduce noise in main program example
    public partial class Program
    {
        internal static readonly string[] YesNoChoice = new[] { "yes", "no" };

        /// <summary>
        /// Instructs ReadValue to accept defaults
        /// </summary>
        internal static bool IsUnitTest { get; set; } = false;

        /// <summary>
        /// Main execute loop to show examples
        /// </summary>
        /// <param name="apiClient"></param>
        /// <param name="queries"></param>
        /// <returns></returns>
        private static async Task Execute(
            HttpClient apiClient,
            IJsonSerializer jsonSerializer,
            List<QueryDescriptor> queries)
        {
            var isStop = false;
            Begin();

            while (!isStop)
            {
                var request =
                    ReadValue("Please enter query by name or number:", string.Empty)
                    .TrimEnd(':');
                isStop = request is null || stopwords.Contains(request);

                if (isStop) { continue; }

                switch (request?.ToLower())
                {
                    case "list":
                    case "help":
                        PrintList(queries);
                        continue;
                    case "reset":
                    case "clear":
                    case "cls":
                        Begin(isReset: true);
                        continue;
                }

                if (!GetQueryByRequest(request,
                    queries,
                    out var query))
                {
                    Console.WriteLine("Unknown query, please use 'list' to see available queries!");
                    End();
                    continue;
                }

                var response = query!.Value.Query.Execute(apiClient);
                var responseSuccess = await response.IsSuccessful();
                Console.WriteLine();
                Console.WriteLine($"{query.Value.DisplayName} {(responseSuccess ? "succeeded" : "failed")}!");
                var httpResponse = await response.GetHttpResponseMessage();
                Console.WriteLine($"{(int)httpResponse.StatusCode}: {httpResponse.RequestMessage?.RequestUri}");
                Console.WriteLine();

                if (responseSuccess
                    && query!.Value.Query is IQueryWithMessageRenderer writer)
                {
                    if (ReadValue("Display content?", string.Empty, YesNoChoice).IsYes())
                    {
                        try
                        {
                            await writer.MessageRenderer.Render(response, jsonSerializer, s => Console.WriteLine(s));
                            Console.WriteLine();
                            Console.WriteLine();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Unable to render content using {writer.MessageRenderer.GetType().FullName}!");
                            Console.WriteLine($"Serializer = {jsonSerializer.GetType().FullName}!");

                            if (ReadValue("Display error?", string.Empty, YesNoChoice).IsYes())
                            {
                                Console.WriteLine(e.ToString());
                            }
                        }
                    }
                }

                if (ReadValue("Display raw response content?", string.Empty, YesNoChoice).IsYes())
                {
                    var stringContent = await (await response.GetHttpResponseMessage())
                        .Content.ReadAsStringAsync();
                    Console.WriteLine("Response message:");
                    Console.WriteLine(stringContent);
                    Console.WriteLine();
                    Console.WriteLine($"Response size ({stringContent.Length.Bytes().Humanize(format: "#.##")}): ");
                    Console.WriteLine();
                }

                End();
            }
        }

        internal static List<QueryDescriptor> GetQueries()
        {
            var queryTypes = typeof(Program)
                .Assembly
                .GetExportedTypes()
                .Where(o => typeof(IQuery).IsAssignableFrom(o) && o.IsClass && !o.IsAbstract)
                .ToArray();
            var queries = new List<QueryDescriptor>(queryTypes.Length);

            foreach (var type in queryTypes)
            {
                if (Activator.CreateInstance(type) is not IQuery intance) { continue; }

                queries.Add(intance.GetQueryDescriptor());
            }

            return queries;
        }

        internal static bool GetQueryByRequest(
            string? request,
            List<QueryDescriptor> queries,
            out QueryDescriptor? query)
        {
            query = null;

            if (request is null) { return false; }

            if (int.TryParse(request, out var queryNumber) && queryNumber > 0)
            {
                query = queries.FirstOrDefault(o => o.QueryNumber == queryNumber);

                return query.Value.IsSet();
            }

            query = queries.FirstOrDefault(o => string.Compare(request, o.DisplayName, ignoreCase: true) == 0);

            return query.Value.IsSet();
        }

        private static void PrintList(List<QueryDescriptor> queries)
        {
            Console.WriteLine();
            Console.WriteLine($"Available queries:");
            Console.WriteLine();

            foreach (var group in queries
                .OrderBy(o => o.QueryNumber)
                .ThenBy(o => o.Group)
                .GroupBy(o => o.Group))
            {
                Console.WriteLine(group.Key);

                foreach (var descriptor in group)
                {
                    Console.WriteLine($"\t{descriptor.QueryNumber} {descriptor.DisplayName}: {descriptor.Description}");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void Begin(bool isReset = false)
        {
            if (isReset) { Console.Clear(); }
            Console.WriteLine("To see queries type: list|help");
            Console.WriteLine("To clear console: clear|reset|cls");
            Console.WriteLine($"To exit type: {string.Join('|', stopwords)}");
            Console.WriteLine();
        }

        private static void End()
        {
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();
        }

        private static readonly HashSet<string> stopwords = new(StringComparer.OrdinalIgnoreCase)
        {
            "q",
            "quit",
            "end",
            "exit",
            "stop",
        };

        public static T ReadValue<T>(
            string prompt,
            T defaultValue,
            System.Collections.Generic.ICollection<string>? choiceList = null)
        {
            if (IsUnitTest) { return defaultValue; }

            Console.Write($"{prompt} {GetDefaultValue(defaultValue)} {GetChoiceListDisplay(choiceList)} ");

            return Console.ReadLine().GetValueOrFallback(defaultValue);
        }

        private static string GetChoiceListDisplay(System.Collections.Generic.ICollection<string>? choices) =>
            choices is null || choices.Count == 0
                ? string.Empty
                : $"({string.Join('|', choices)})";

        private static string GetDefaultValue<T>(T defaultValue) =>
            defaultValue is string s && string.IsNullOrWhiteSpace(s)
                ? string.Empty
                : $"(default: {defaultValue})";
    }
}