public class Levenshtein
{
    //aquí hallo la sugerencia
    public static (string, string []) SimilitudPalabras(List<string> quer1,List<string> lista_sinrepeticion, string query)
    {
        //array para guardar los calculos de levenshtein
        double[] cl = new double[lista_sinrepeticion.Count];
        //array para guardar las palabras que tengan mayor semejanza
        string[] sugerencias = new string[quer1.Count];
        //array para guardar las sugerencias con los operadores
        string[] sug_op = new string[sugerencias.Length];
        double distancia;
        for (int i = 0; i < quer1.Count; i++)
        {
            for (int j = 0; j < lista_sinrepeticion.Count; j++)
            {
                //comparando la palabra del query con la lista de todas las palabras de los documentos
                distancia = Calculo(quer1[i], lista_sinrepeticion[j]);
                //almancenar el el array cl todas las distancias calculadas
                cl[j] = distancia;
            }
            //guardar las palabras que tengan menor ditancia de levenshtein
            sugerencias[i] = lista_sinrepeticion[MenorDist(cl)];
        }
        //trabajar con la relacion entre los operadores y la sugerencia
        for (int i = 0; i < sugerencias.Length; i++)
        {
            sug_op[i] = sugerencias[i];
        }
        string palabra = "";  int k = 0;
        for (int j = 0; j < query.Length; j++)
        {
            if (query[j] == ' ' && palabra == "" || query[j] == '~') continue;
            if (Char.IsLetterOrDigit(query[j]) || query[j] == '*' || query[j] == '!' || query[j] == '~' || query[j] == '^')
            {
                palabra += query[j];
            }
            else if (query[j] == ' ' || palabra != "")
            {
                if (palabra[0] == '!') sug_op[k] = '!' + sugerencias[k];
                if (palabra[0] == '*') sug_op[k] = '*' + sugerencias[k];
                if (palabra[1] == '*') sug_op[k] = "**" + sugerencias[k];
                if (palabra[0] == '^') sug_op[k] = '^' + sugerencias[k];
                k++;  palabra = "";
            }
        }
        string sugerencia_op = "";
        for (int i = 0; i < sug_op.Length; i++)
        {
            sugerencia_op += sug_op[i] + " ";
        }

        return (sugerencia_op, sugerencias);
    }
    //método para obtener la menor distancia de levenshtein entre una palabra del query y la lista de todas
    //las palabras, mientras menor sea la distancia mayor sera la similitud de las palabras
    private static int MenorDist(double[] cl)
    {
        double min = double.MaxValue;  int indice = 0;
        for (int i = 0; i < cl.Length; i++)
        {
            if (cl[i] < min)
            {
                min = cl[i];
                indice = i;
            }
        }
        return indice;
    }
    private static double Calculo(string palabra1, string palabra2)
    {
        int costo = 0;
        //matriz donde las filas representa la palabra introducida en la query y las columnas
        //las palabra del documento con la que se compara la palabra de la query 
        int[,] tabla = new int[palabra1.Length + 1, palabra2.Length + 1];
        //Llena la primera columna y la primera fila.
        for (int i = 0; i <= palabra1.Length; tabla[i, 0] = i++) ;
        for (int h = 0; h <= palabra2.Length; tabla[0, h] = h++) ;
        /// recorre la matriz llenando cada unos de los pesos.
        /// i filas, j columnas
        for (int i = 1; i <= palabra1.Length; i++)
        {
            for (int j = 1; j <= palabra2.Length; j++)
            {
                //si las letras son iguales en iguales posiciones entonces el costo es 0
                //si son diferentes el costo es 1
                costo = (palabra1[i - 1] == palabra2[j - 1]) ? 0 : 1;
                //eliminación, inserción y sustitución
                tabla[i, j] = Math.Min(Math.Min(tabla[i - 1, j] + 1, 
                tabla[i, j - 1] + 1),                                     
                tabla[i - 1, j - 1] + costo);                                  
            }
        }
        //Calculamos el porcentaje de cambios en la palabra
        if (palabra1.Length > palabra2.Length) return ((double)tabla[palabra1.Length, palabra2.Length] / (double) palabra1.Length);
        else return ((double)tabla[palabra1.Length, palabra2.Length] / (double)palabra2.Length);
    }
}