using CronJobsTranslator.CronJobs;
using CronJobsTranslator.Data;
using Cronos;
using System.Security.Cryptography;

// Argentina: GMT-3 fijo, sin horario de verano.
// "America/Argentina/Buenos_Aires" es el ID de IANA (funciona en Linux/Mac).
// En Windows con TimeZoneInfo "viejo" sería "Argentina Standard Time".
var zonaArgentina = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");

Console.WriteLine($"Símbolos que aparecen en la lista:");
Console.WriteLine($"");
Console.WriteLine($"* significa \"cualquier valor\"(todos los minutos, todas las horas, etc.)");
Console.WriteLine($", separa una lista de valores puntuales — 6,13 significa \"a las 6 y a las 13\"");
Console.WriteLine($"/ indica un intervalo o \"step\" — */5 significa \"cada 5 unidades, empezando desde 0\"");
Console.WriteLine($"- define un rango — 1-5 significa \"del 1 al 5\" (lunes a viernes, si está en el campo de día de semana)");
Console.WriteLine($"");
//Console.WriteLine($"{"Expresión cron",-15}{"Próxima ejecución UTC",-29}{"Próxima ejecución ARG (GMT-3)",-35}{"Suspendido",-12}{"Detalle del CronJob",-20}");
Console.WriteLine($"{"Expresión cron",-16}{"Detalle del CronJob",-80}{"Servicio del CronJob",-20}");
Console.WriteLine($"┌───────────── minuto(0 - 59)");
Console.WriteLine($"│ ┌───────────── hora(0 - 23)");
Console.WriteLine($"│ │ ┌───────────── día del mes(1 - 31)");
Console.WriteLine($"│ │ │ ┌───────────── mes (1 - 12)");
Console.WriteLine($"│ │ │ │ ┌───────────── día de la semana (0 - 6, donde 0 = domingo)");
Console.WriteLine($"│ │ │ │ │");
Console.WriteLine($"* * * * *");

Console.WriteLine(new string('-', 200));

foreach (var cronjob in CronJobsSeed.GetAll())
{
    // El segundo parámetro indica si la expresión tiene campo de segundos.
    // Las cron clásicas de 5 campos no lo tienen.
    var cronExpression = CronExpression.Parse(cronjob.Horario);

    // GetNextOccurrence calcula la próxima ejecución a partir de "ahora" en UTC.
    DateTime? proximaUtc = cronExpression.GetNextOccurrence(DateTime.UtcNow);

    if (proximaUtc is null)
    {
        Console.WriteLine($"{cronjob,-20} No se pudo calcular (expresión inválida o sin próxima ocurrencia)");
        continue;
    }

    // Convertimos de UTC a la hora de Argentina
    DateTime proximaArg = TimeZoneInfo.ConvertTimeFromUtc(proximaUtc.Value, zonaArgentina);

    var cronToText = CronToText.Describir(CronJobsSeed.GetCronExpression(cronjob));

    //Console.WriteLine($"{CronJobsSeed.GetCronExpression(cronjob),-16}{"UTC "}{proximaUtc:yyyy-MM-dd HH:mm:ss}{"",5}{"  ARG "}{proximaArg:yyyy-MM-dd HH:mm:ss}{"",12}{CronJobsSeed.GetSuspend(cronjob),-12}{CronJobsSeed.GetCronJob(cronjob)}");
    Console.WriteLine($"{CronJobsSeed.GetCronExpression(cronjob),-16}{cronToText,-42}{"  ARG "}{proximaArg:yyyy-MM-dd HH:mm:ss}{"",3}{CronJobsSeed.GetEnvironment(cronjob), -11}{CronJobsSeed.GetNamespace(cronjob),-22}{CronJobsSeed.GetShortEndpointServicio(cronjob)}");
}