using CronJobsTranslator.CSVHelper;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Linq.Expressions;

namespace CronJobsTranslator.Data
{
    public static class CronJobsSeed
    {
        public static string GetCronExpression(Expresion expresion)
        {
            return expresion.Horario;
        }
        public static string GetCronJob(Expresion expresion)
        {
            return ""
                + expresion.Cronjob 
                ;
        }

        public static string GetNamespace(Expresion expresion)
        {
            return ""
                + expresion.Namespace
                ;
        }

        public static string GetEnvironment(Expresion expresion)
        {
            var env = expresion.Namespace.Split("-").LastOrDefault(x => (x == "dev" || x == "qa" || x == "av"));
            return ""
                + (expresion.Ambiente)
                + "(" 
                + (env is null ? "prd" : env) 
                + ")"
                ;
        }

        public static string GetSuspend(Expresion expresion)
        {
            return (expresion.Suspend ? "SI" : "NO");
        }

        public static string GetEndpointServicio(Expresion expresion)
        {
            return ""
                + expresion.Verbo
                + " " 
                + expresion.EndpointServicio
                ;
        }

        public static string GetShortEndpointServicio(Expresion expresion)
        {
            return ""
                + expresion.Verbo
                + " " 
                + expresion.ShortEndpointServicio
                ;
        }

        /// <summary>
        /// Ruta por defecto del CSV. Ajustar según dónde se despliegue el archivo
        /// (por ejemplo, junto al ejecutable, o como Content/EmbeddedResource).
        /// </summary>
        //private const string RutaCsvPorDefecto = "C:\\Users\\User0132\\source\\repos\\CronJobsTranslator\\CronJobsTranslator\\Data\\cronjobs.csv";
        private const string RutaCsvPorDefecto = "Data\\cronjobs.csv";

        /// <summary>
        /// Lee el CSV y devuelve la lista de Expresion.
        /// Mantiene la misma firma que el GetAll() original para no romper el código existente.
        /// </summary>
        public static List<Expresion> GetAll() => GetAll(RutaCsvPorDefecto);

        /// <summary>
        /// Sobrecarga que permite indicar una ruta de archivo distinta (útil para tests o configuración).
        /// </summary>
        public static List<Expresion> GetAll(string rutaArchivoCsv)
        {
            if (!File.Exists(rutaArchivoCsv))
                throw new FileNotFoundException($"No se encontró el archivo CSV de cronjobs en '{rutaArchivoCsv}'.");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null, // tolera columnas ausentes sin lanzar excepción
            };

            using var reader = new StreamReader(rutaArchivoCsv);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ExpresionCsvMap>();

            return csv.GetRecords<Expresion>().ToList();
        }

        /// <summary>
        /// Variante que lee el CSV desde un Stream (útil si el archivo está embebido como
        /// EmbeddedResource en el assembly, en vez de copiarse como archivo suelto).
        /// </summary>
        public static List<Expresion> GetAllFromStream(Stream csvStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
            };

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ExpresionCsvMap>();

            return csv.GetRecords<Expresion>().ToList();
        }
    }
}