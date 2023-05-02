public static class TFIDF
{
   public static Dictionary <string, double> [] tfidf (List<List<string>> todas_palabras, Dictionary<string, int> [] palabras_repeticion, string [] direcciones)
    {
        //diccionario para guardar el TF*IDF
        Dictionary<string, double> [] tfidf = new Dictionary<string, double>[palabras_repeticion.Length];
        for (int i = 0; i < palabras_repeticion.Length; i ++ )
        {
            //inicializar el diccionario
            tfidf[i] = new();
            //convertir el diccionario en lista para obtener su value y key posteriormente
            var lista = palabras_repeticion[i].ToList();
            //esto almacena # de palabras totales de cada documento
            int tp = todas_palabras[i].Count;
            for (int j = 0; j < palabras_repeticion[i].Count; j ++)
            {
                //contador para almacenar lo cantidad de documentos que contienen a una
                //determinada palabra
                int contador = 0;
                var termino = lista[j].Key;
                for (int k = 0; k < palabras_repeticion.Length; k ++)
                {
                if (palabras_repeticion[k].ContainsKey(termino)) contador++;
                }
                //fórmula del TF, TF = veces q aparece una palabra en un doc/ # de palabras totales del doc
                // tf = lista[j].Value/tp
                //fórmula de IDF, IDF = cantidad de docs/# de docs que contienen a la palabra
                //idf = Math.Log10 (direcciones.Length/contador)
                tfidf[i].Add(termino, (double) lista[j].Value/ (double) tp * (double) (Math.Log10(direcciones.Length/(double) contador + 0.000001)));
            }   
        }
     return tfidf;
    }
    //el proximo método hace el mismo proceso del método LeerPalabras, pero con la query
    public static List<string> Query (string query)
    {
        List<string> query1 = new();
        string palabra = "";
        //Con este for quitamos todo lo q no sea letra o número
        for (int j = 0; j < query.Length; j++)
        {
            if (query[j] == ' ' && palabra == "") continue;
            if (Char.IsLetterOrDigit(query[j])) palabra += query[j];
            else if  (query[j] == ' ' || palabra != "") 
            {
            query1.Add(palabra.ToLower()); 
            palabra = "";
            }
        }
        //por si quedo una palabra en memoria
        if (palabra != "") query1.Add(palabra.ToLower());
        return query1;
    }
   public static double[] TFIDF_Query (List<string> query_, Dictionary<string, int> [] palabras_repeticion)
    {
        //inicializar el diccionario para guardar el tfidf del query
        Dictionary <string, double> tfidf_query = new Dictionary<string, double> ();
        //array para guardar la cantidad de veces que se repite una palabra en el query
        int [] repeticion = new int [query_.Count];
        //inicializar el diccionario
        tfidf_query = new ();
        //recorriendo la query para almacenar el array de int las repeticiones
        for (int i = 0; i < query_.Count; i ++)
        {
            int cont = 0;
            for (int j = 0; j < query_.Count; j ++)
            {
                if (query_[i] == query_[j]) cont ++;
            }
            repeticion[i] = cont;
        }
        for (int i = 0; i < query_.Count; i ++)
        {
            //cantidad de palabras que tiene la query
            int pt = query_.Count;
            //contador para almacenar los documentos que contienen la palabra del query
            int cont1 = 0;
            var término = query_[i];
            for (int j = 0; j < palabras_repeticion.Length; j ++)
            {
                if (palabras_repeticion[j].ContainsKey(término)) cont1 ++;
            }
            double resultado = (double) repeticion[i] / (double) pt * (double) Math.Log10(palabras_repeticion.Length/ (double) (cont1 + 0.00001));
            if (tfidf_query.ContainsKey(término)) continue;
            else tfidf_query.Add(término, resultado);
            //calculando TF*IDF de la query
            //tf = repetición de una palabra en la query/cant de palabras de la query
            //idf = Log10 (cantidad total de documentos/# de documentos en los que aparece la palabra de la query)
            //al contador le sumo un numero muy pequeño pues habían ocasiones en las cuales algunas
            //palabras me daban un tfidf igual a infinito🙄🤔
            //tfidf_query.Add(término, (double) repeticion[i] / (double) pt * (double) Math.Log10(palabras_repeticion.Length/ (double) (cont1 + 0.00001)));    
        }
       double [] vector_query = tfidf_query.Values.ToArray(); 
       return vector_query;
    }
}