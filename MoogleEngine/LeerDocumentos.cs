public class LeerDocumentos 
{
   public static (string [], string []) LeerDocs ()
    {
        //ruta de la carpeta donde estoy parada
        var rutactual = Directory.GetParent(Directory.GetCurrentDirectory());
        //direcciones de las documentos que están en la carpeta content
        string [] direcciones = Directory.GetFiles(rutactual + @"\Content");
        //obtener el nombre de estos documentos, eliminando la ruta
        string [] nombres = new string [direcciones.Length];
        for (int i = 0; i < direcciones.Length; i ++)
        {
            //utilizando el método GetFileName 
            string nombredocs = Path.GetFileName(direcciones[i]);
            nombres[i] += nombredocs;;
        } 
        return (direcciones, nombres); 
    }
    public static (Dictionary<string, int> [], List<List<string>>, List<string>) LeerPalabras (string [] documentos)
    {
        //lista de listas para guardar todas las palabras de los documentos, con repetición (sirve para calcular el tf)
        List<List<string>> todas_palabras = new List<List<string>> ();
        //lista para almacenar todas las palabras sin repeticion, se utilizara en el calculo
        //del Levenshtein
        List<string> lista_sinrepeticion = new List<string>();
        //diccionario para guardar las palabras por documentos y su repetición
        Dictionary<string, int> [] palabras_repeticion = new Dictionary<string, int> [documentos.Length];
        //string para guardar las palabras
        string p = "";
        //for para leer las palabras de los documentos
        for (int i = 0; i < documentos.Length; i ++)
        {
            //iniciar la lista y diccionario
            todas_palabras.Add(new());
            palabras_repeticion[i] = new();
            //streamreader para leer las palabras de los documentos (no me reconocía algunos caracteres 
            //por lo que tuve que convertir el texto a system.text.encoding.UTF7)
            StreamReader leerpalabras = new StreamReader(documentos[i], System.Text.Encoding.UTF7);
            //que lea hasta el último caracter y convertir el texto a minúsculas
            string docs = leerpalabras.ReadToEnd().ToLower();
            //for para analizar las palabras de los textos
            for (int j = 0; j < docs.Length; j ++)
            {
                //si hay espacio vacío, continuar
                if (docs[j] == ' ' && p == "") continue;
                //si hay una letra o número, aumentar palabra (p)
                if (Char.IsLetterOrDigit(docs[j])) p += docs[j];
                else if (docs[j] == ' ' || p != "" ) 
                {
                    //si luego hay un espacio vacío y (p) se completó, añadir la palabra a la lista de listas 
                    todas_palabras[i].Add(p);
                    //rellenar la lista
                    if (!lista_sinrepeticion.Contains(p)) lista_sinrepeticion.Add(p);
                    //rellenar el diccionario
                    if ((palabras_repeticion[i].ContainsKey(p))) 
                    {
                       palabras_repeticion[i][p]++; 
                    }
                    else 
                    {
                        palabras_repeticion[i].Add(p, 1);
                    }
                    p = "";
                }
            }
            //la última palabra no va a entrar en el anterior else if, pero si aquí
            if (p != "")
            {
              string pl = p.ToLower();
              todas_palabras[i].Add(pl);
              if (!lista_sinrepeticion.Contains(p)) lista_sinrepeticion.Add(p);
              if (palabras_repeticion[i].ContainsKey(pl))
              {
                palabras_repeticion[i][pl]++;
              }
              else
              {
                palabras_repeticion[i].Add(pl, 1);
              }
              pl = "";
            }
            //concluir la lectura de los documentos
            leerpalabras.Close();
        }
        return (palabras_repeticion, todas_palabras,  lista_sinrepeticion);
    }
}