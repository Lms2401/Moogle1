namespace MoogleEngine;


public static class Moogle
{
     public static SearchResult Query(string query)
    {
        //llamado a las clases y métodos principales
        string [] documentos = LeerDocumentos.LeerDocs().Item2;
        string [] direcciones = LeerDocumentos.LeerDocs().Item1;
        List<List<string>> todas_palabras = LeerDocumentos.LeerPalabras(direcciones).Item2;
        List<string> lista_sinrepeticion = LeerDocumentos.LeerPalabras(direcciones).Item3;
        Dictionary<string, int> [] palabras_repeticion = LeerDocumentos.LeerPalabras(direcciones).Item1;
        Dictionary<string, double> [] tfidf = TFIDF.tfidf(todas_palabras, palabras_repeticion, documentos);
        List<string> quer1 = TFIDF.Query(query);
        double [] vectorquery = TFIDF.TFIDF_Query(quer1, palabras_repeticion);
        double [,] matriz = SimilitudCoseno.Matriz(quer1, tfidf);
        double [] score = SimilitudCoseno.SimCos(matriz, vectorquery);
        string [] levenshtein = Levenshtein.SimilitudPalabras(quer1, lista_sinrepeticion, query).Item2;
        string levenshtein_op = Levenshtein.SimilitudPalabras(quer1, lista_sinrepeticion, query).Item1;
        double [] SimCos_Op = Operadores.MetodoPrincipal(score, todas_palabras, palabras_repeticion, levenshtein, query);
        string [] snippet = Snippet.Snippet1(todas_palabras, quer1, SimCos_Op);
        //la próxima parte del código es el algorítimo para ordenar los documentos desde el que 
        //mayor score tiene hasta el menor
        for (int i = 0; i < score.Length; i ++)
        {
            for (int j = i; j < score.Length; j ++)
            {
                if (SimCos_Op[i] < SimCos_Op[j])
                {
                    double MayorSimCos = SimCos_Op[j];  string doc = documentos[j];      string newsnippet = snippet[j];
                    SimCos_Op[j] = SimCos_Op[i];        documentos[j] = documentos[i];   snippet[j] = snippet[i];
                    SimCos_Op[i] = MayorSimCos;         documentos[i] = doc;             snippet[i] = newsnippet; 
                }
            }
        }
        //si el documento tiene una simcos igual a 0, entonces no se tendrá en cuenta
        int cont = 0;
        for (int i = 0; i < score.Length; i ++)
        {
            if (score[i] != 0) cont ++;
        }
        double [] newscore = new double [cont];   string [] newdocs = new string [cont]; 
        for (int i = 0; i < newscore.Length; i ++)
        {
            newscore[i] = SimCos_Op[i];
        }
        SearchItem[] items = new SearchItem[newscore.Length];
        for (int i = 0; i < newscore.Length; i ++)
        {
            items[i] = new SearchItem (documentos[i], snippet[i], SimCos_Op[i]);
        }
        return new SearchResult (items, levenshtein_op == query ? "" : levenshtein_op);
    }
}
