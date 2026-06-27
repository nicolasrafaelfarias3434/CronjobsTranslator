using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace CronJobsTranslator.CSVHelper
{
    // =========================================================
    // 3) Repositorio que reemplaza al GetAll() hardcodeado
    // =========================================================
    public static class ExpresionRepository
    {
        /// <summary>
        /// Ruta por defecto del CSV. Ajustar según dónde se despliegue el archivo
        /// (por ejemplo, junto al ejecutable, o como Content/EmbeddedResource).
        /// </summary>
        private const string RutaCsvPorDefecto = "Data/cronjobs.csv";

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
