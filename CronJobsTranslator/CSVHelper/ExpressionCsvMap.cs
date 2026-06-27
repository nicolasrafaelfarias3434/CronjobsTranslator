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
    // 2) Mapeo CSV -> Expresion (CsvHelper)
    //    Header esperado en el CSV:
    //    Ambiente,Namespace,Cronjob,Verbo,EndpointServicio,ShortEndpointServicio,Horario,Suspend
    // =========================================================
    public sealed class ExpresionCsvMap : ClassMap<Expresion>
    {
        public ExpresionCsvMap()
        {
            Map(m => m.Ambiente).Name("Ambiente");
            Map(m => m.Namespace).Name("Namespace");
            Map(m => m.Cronjob).Name("Cronjob");
            Map(m => m.Verbo).Name("Verbo");
            Map(m => m.EndpointServicio).Name("EndpointServicio");
            Map(m => m.ShortEndpointServicio).Name("ShortEndpointServicio");
            Map(m => m.Horario).Name("Horario");
            Map(m => m.Suspend).Name("Suspend");
        }
    }
}
