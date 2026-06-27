using System;
using System.Collections.Generic;
using System.Text;

namespace CronJobsTranslator.CSVHelper
{
    public class Expresion
    {
        public string Ambiente { get; set; }
        public string Namespace { get; set; }
        public string Cronjob { get; set; }
        public string Verbo { get; set; }
        public string EndpointServicio { get; set; }
        public string ShortEndpointServicio { get; set; }
        public string Horario { get; set; }
        public bool Suspend { get; set; }
    }
}
