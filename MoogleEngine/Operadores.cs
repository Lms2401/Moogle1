public class Operadores
{
    //método que trabaja con todos los operadores, la query, la similitud del coseno y la sugerencia
    public static double [] MetodoPrincipal (double [] simCos, List<List<string>> todas_palabras, Dictionary<string, int> [] palabras_repeticion, string [] sugerencia, string query)
    {
        //lista para guardar las palabras, incluso con operadores
        List<string> query_operadores = new();
        int k = -1;  var concatenar = "";  string palabra = "";  int cantAsterisco = 0;
        //con este for guardamos las palabras de la query con sus operadores, y tambien trabajamos con los posteriores métodos
        for (int j = 0; j < query.Length; j++)
        {
            if (query[j] == ' ' && palabra == "") continue;
            //si existe una letra, incluso un operador, guardalos en el string palabra
            if (Char.IsLetterOrDigit(query[j]) || query[j] == '*' || query[j] == '!' || query[j] == '~' || query[j] == '^')
            {
                palabra += query[j];
            }
            //cuando ya se haya guardado la palabra, llamar a los métodos posteriores para que trabajen con los operadores
            else if (query[j] == ' ' || palabra != "")
            {
                if (palabra.ToCharArray()[1] == '~') query_operadores.Add(palabra);
                else
                {
                  //la variable k se utiliza para hacer referencia a la sugerencia (posición de la palabra sugerida que nos interesa)
                  k++; query_operadores.Add(palabra);
                  if (palabra[0] == '!') NoAparece(sugerencia[k], palabras_repeticion, simCos);
                  if (palabra[0] == '^') Aparece(sugerencia[k], palabras_repeticion, simCos);
                  //mayor o menor prioridad en dependencia de la cantidad de asteriscos
                  if (palabra[0] == '*') 
                  {
                    for (int i = 0; i < palabra.Length; i++)
                    {
                        if (palabra[i] == '*') cantAsterisco++;
                        else break;
                    }
                   Relevancia (sugerencia[k], palabras_repeticion, cantAsterisco, simCos);
                  }
                }
                //limpiar el string palabra y la variable asterisco para evitar concatenaciones o sumas
                palabra = "";
            }
            cantAsterisco = 0;
        }
        if (palabra != "")
        {
            query_operadores.Add(palabra); k++;
            if (palabra[0] == '!') NoAparece(sugerencia[k], palabras_repeticion, simCos);
            if (palabra[0] == '^') Aparece(sugerencia[k], palabras_repeticion, simCos);
            if (palabra[0] == '*')
            {
                for (int i = 0; i < palabra.Length; i++)
                {
                    if (palabra[i] == '*') cantAsterisco++;
                    else break;
                }
                Relevancia (sugerencia[k], palabras_repeticion, cantAsterisco, simCos);
            }  
        }
        //trabajando con la cercanía (~)
        for (int i = 0; i < query_operadores.Count; i++)
        {
            //une todas las palabras de la query con operadores incluidos
            concatenar += query_operadores[i];
        }
     //transformarlo en una lista de char
     var lista_concatenar = concatenar.ToList();
     if (lista_concatenar.Contains('~')) Cercania(query_operadores, todas_palabras, simCos);
     return simCos;
    }
    //método que trabaja con el operador (!)
    private static void NoAparece (string palabra_sug_quey, Dictionary<string, int>[] palabras_repeticion, double[] simCos)
    {
        for (int i = 0; i < palabras_repeticion.Length; i++)
        {
            if (palabras_repeticion[i].ContainsKey(palabra_sug_quey)) simCos[i] = 0;
        }
    }
    //método que trabaja con el operador (*) 
    private static void Relevancia (string palabra_sug_quey, Dictionary<string, int>[] palabras_repeticion, int cant_asteriscos, double [] simCos)
    {
        for (int i = 0; i < palabras_repeticion.Length; i++)
        {
            //cuando la palabra aparece con el símbolo *, tendrá mayor score, mientras más asteriscos hayan
            //mayor será el score 
            if (palabras_repeticion[i].ContainsKey(palabra_sug_quey)) simCos[i] *= cant_asteriscos + 1;
        }
    }
    //método que trabaja con el operador (^)
    private static void Aparece (string palabra_sug_quey, Dictionary<string, int>[] palabras_repeticion, double[] simCos)
    {
        for (int i = 0; i < palabras_repeticion.Length; i++)
        {
            //cuando la palabra aparece con el signo ^, entonces devolverá el documento donde aparezca
            if (!palabras_repeticion[i].ContainsKey(palabra_sug_quey)) simCos[i] = 0;
        }
    }
    //metodo que modifica el score en dependencia de la distancia que haya entre dos palabras
    public static void Cercania(List<string> queryOp, List<List<string>> lista, double[] simCos)
    {
     string palabra1 = "";  
     string palabra2 = "";
     int indice = 0;
     List<int> indices = new List<int>();
     //recorre la lista que contiene las palabras de la query con operadores para obtener el indice de
     //las palabras que contengan al operador ~
     for (int i = 0; i < queryOp.Count; i++)
     {
      for (int j = 0; j < queryOp[i].Length; j++)
      {
        if (queryOp[i][j] == '~')
        {
         indice = i;
         indices.Add(indice);
        }
      }
     }
     int [] indarray = indices.ToArray();
     string [] arrayp = new string [indarray.Length];
     string [] querops = queryOp.ToArray();
     int cont = 0; int num = 0; int cont2 = 0;
     int menor = int.MaxValue; int indice3 = 0;
     //agregar a un array las palabras de la query que contengan al operador ~, a traves del array
     //de indices creado anteriormente
     for (int i = 0; i < indarray.Length; i++)
     {
        arrayp[i] = querops[indarray[i]];   
     }

     for (int i = 0; i < arrayp.Length; i++)
     {
        //con este for, tomamos las dos palabras que tienen a (~) en el medio
        for (int j = 0; j < arrayp[i].Length; j++)
        {
         if (char.IsLetterOrDigit(arrayp[i][j]) && cont == 0)
         {
            palabra1 += arrayp[i][j];
         }
         if (cont == 1 && Char.IsLetterOrDigit(arrayp[i][j]))
         {
            palabra2 += arrayp[i][j];
         }
         if (palabra1 != "" && arrayp[i][j] == '~')
         {
          cont ++;
         }
        }
        //con este for analizamos si esas palabras se encuentran en los documentos
        //y si se encuentran nos quedamos con el documento que tenga la menor distancia entre ellas
        for (int k = 0; k < lista.Count; k++)
        {
            if (lista[k].Contains(palabra1) && lista[k].Contains(palabra2))
            {
              cont2 ++;
              num = MenorDist(palabra1,palabra2,lista[k]);
              if (num < menor)
              {
                menor = num;
                indice3 = k;
              }
            }   
        }
        //nos quedamos con el indice del documentos donde se encuentra la menor distancia 
        //para luego poder modificar su score y aumentarlo
        if (cont2 != 0)
        {
        simCos[indice3] += 2;
        }
       palabra1 = ""; palabra2 = ""; cont = 0;  menor = int.MaxValue; cont2 = 0;
     }
    }
    //metodo que utiliza el metodo anterior para calcular la distancia entre dos palabras
    public static int MenorDist (string palabra1, string palabra2, List<string> doc)
    {
     int indice1 = 0; int indice2 = 0;
     for (int i = 0; i < doc.Count; i++)
     {
      if (palabra1 == doc[i])
      {
       indice1 = i;
      } 
      if (palabra2 == doc[i])
      {
        indice2 = i;
      } 
     }
     int resultado = Math.Abs(indice1 - indice2);
     return resultado;
    }
    }
    