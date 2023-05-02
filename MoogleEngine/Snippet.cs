public class Snippet
{
    public static string BuscarSnippet(List<string> lista, string palabra, int i, double[] simCos)
    {
        //si la similitud del coseno de ese documento es 0, pues no calculamos su snippet
        if (simCos[i] == 0) return "";
        //sino pues lo hallamos
        else
        {
            string snippet = ""; string partei = ""; string partef = "";
            for (int j = 0; j < lista.Count; j++)
            {
                if (lista[j] == palabra && snippet == "")
                {
                    int min_palabras = 15;
                    for (int k = j; k >= 0; k--)
                    {
                        snippet = lista[k] + " " + partei;
                        min_palabras--;
                        if (min_palabras == 0) break;
                    }
                    int max_palabras = 14;
                    for (int k = j; k < lista.Count; k++)
                    {
                        partef += " " + lista[k];
                        max_palabras--;
                        if (max_palabras == 0) break;
                    }
                    snippet = partei + partef;
                }
            }
            return snippet;
        }
    }
    //metodo para llamar al metodo Snippet y guardar cada snippet en un array de strings
    public static string [] Snippet1 (List<List<string>> todas_palabras, List<string> query, double[] simCos)
    {
        string[] snipp = new string[todas_palabras.Count];
        for (int j = 0; j < query.Count; j++)
        {
            for (int i = 0; i < todas_palabras.Count; i++)
            {
                if (todas_palabras[i].Contains(query[j]))
                {
                    snipp[i] = BuscarSnippet(todas_palabras[i], query[j], i, simCos);
                }
            }
        }
        return snipp;
    }
}