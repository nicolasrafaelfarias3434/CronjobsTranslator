using System;
using System.Collections.Generic;
using System.Linq;

namespace CronJobsTranslator.CronJobs;

public static class CronToText
{
    private static readonly string[] DiasSemana =
    {
        "domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"
    };

    public static string Describir(string cronExpression)
    {
        if (string.IsNullOrWhiteSpace(cronExpression))
            return "Expresión cron vacía";

        var partes = cronExpression.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length != 5)
            return $"Expresión cron inválida: '{cronExpression}'";

        string minuto = partes[0];
        string hora = partes[1];
        string dia = partes[2];
        string mes = partes[3];
        string diaSemana = partes[4];

        // Caso: cada N minutos (*/N o N/M en el campo de minutos, resto en *)
        if (EsIntervalo(minuto, out int intervaloMin) &&
            hora == "*" && dia == "*" && mes == "*" && diaSemana == "*")
        {
            return intervaloMin == 1
                ? "Se ejecuta cada minuto"
                : $"Se ejecuta cada {intervaloMin} minutos";
        }

        // Caso: cada N horas (minuto fijo, hora */N)
        if (EsValorFijo(minuto, out int minFijoH) &&
            EsIntervalo(hora, out int intervaloHora) &&
            dia == "*" && mes == "*" && diaSemana == "*")
        {
            return $"Se ejecuta cada {intervaloHora} horas, en el minuto {minFijoH:00}";
        }

        // A partir de aquí: asumimos minuto y hora con valores fijos o listas/rangos
        string horaTexto = DescribirHoras(minuto, hora);

        string sufijoDia = DescribirDiaSemana(diaSemana);
        string sufijoFecha = DescribirDiaMes(dia, mes);

        var resultado = $"Se ejecuta {horaTexto}";

        if (!string.IsNullOrEmpty(sufijoDia))
            resultado += $", {sufijoDia}";

        if (!string.IsNullOrEmpty(sufijoFecha))
            resultado += $", {sufijoFecha}";

        return resultado;
    }

    private static bool EsIntervalo(string campo, out int intervalo)
    {
        intervalo = 0;
        if (campo.StartsWith("*/") && int.TryParse(campo.Substring(2), out intervalo))
            return true;
        if (campo.Contains("/") && campo.StartsWith("0") && int.TryParse(campo.Split('/')[1], out intervalo))
            return true; // caso "0/5"
        return false;
    }

    private static bool EsValorFijo(string campo, out int valor)
    {
        return int.TryParse(campo, out valor);
    }

    private static string DescribirHoras(string minutoCampo, string horaCampo)
    {
        var minutos = ExpandirCampo(minutoCampo, 0, 59);
        var horas = ExpandirCampo(horaCampo, 0, 23);

        if (minutos.Count == 1 && horas.Count == 1)
        {
            return $"a las {horas[0]:00}:{minutos[0]:00}";
        }

        if (minutos.Count == 1 && horas.Count > 1)
        {
            var horasTexto = string.Join(", ", horas.Select(h => $"{h:00}:{minutos[0]:00}"));
            return $"a las {horasTexto}";
        }

        return $"según el patrón minuto='{minutoCampo}' hora='{horaCampo}'";
    }

    private static string DescribirDiaSemana(string campo)
    {
        if (campo == "*")
            return null;

        // Rango: 1-5
        if (campo.Contains("-"))
        {
            var partesRango = campo.Split('-');
            if (int.TryParse(partesRango[0], out int inicio) && int.TryParse(partesRango[1], out int fin))
            {
                return $"de {DiasSemana[inicio % 7]} a {DiasSemana[fin % 7]}";
            }
        }

        // Lista: 1,3,5
        if (campo.Contains(","))
        {
            var dias = campo.Split(',')
                .Select(d => int.TryParse(d, out int n) ? DiasSemana[n % 7] : d);
            return $"los días {string.Join(", ", dias)}";
        }

        // Valor único
        if (int.TryParse(campo, out int diaNum))
        {
            return $"todos los {DiasSemana[diaNum % 7]}";
        }

        return $"según el patrón de día de semana '{campo}'";
    }

    private static string DescribirDiaMes(string diaCampo, string mesCampo)
    {
        bool diaEsTodos = diaCampo == "*";
        bool mesEsTodos = mesCampo == "*";

        if (diaEsTodos && mesEsTodos)
            return null;

        if (!diaEsTodos)
            return $"el día {diaCampo} del mes";

        if (!mesEsTodos)
            return $"en el mes {mesCampo}";

        return null;
    }

    /// <summary>
    /// Expande un campo cron a la lista de valores que representa (soporta *, N, N-M, N,M,..., */N)
    /// </summary>
    private static List<int> ExpandirCampo(string campo, int min, int max)
    {
        var resultado = new List<int>();

        if (campo == "*")
        {
            resultado.Add(min); // representativo; no se usa en listas largas
            return resultado;
        }

        foreach (var parte in campo.Split(','))
        {
            if (parte.Contains("/"))
            {
                var seg = parte.Split('/');
                int paso = int.Parse(seg[1]);
                int inicio = seg[0] == "*" ? min : int.Parse(seg[0]);
                for (int i = inicio; i <= max; i += paso)
                    resultado.Add(i);
            }
            else if (parte.Contains("-"))
            {
                var seg = parte.Split('-');
                int inicio = int.Parse(seg[0]);
                int fin = int.Parse(seg[1]);
                for (int i = inicio; i <= fin; i++)
                    resultado.Add(i);
            }
            else
            {
                resultado.Add(int.Parse(parte));
            }
        }

        return resultado;
    }
}