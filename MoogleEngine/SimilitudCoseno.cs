public class SimilitudCoseno
{
    public static double [,] Matriz (List<string> query_, Dictionary<string, double> [] tfidf)
    {
      //creando la matriz, filas = cant de documentos, columnas = cant de palabras del query
      double [,] matriz = new double [tfidf.Length, query_.Count];
      for (int i = 0; i < tfidf.Length; i ++)
      {
          for (int j = 0; j < query_.Count; j ++)
          {
            var palabra = query_[j];
            if (tfidf[i].ContainsKey(palabra)) matriz[i,j] = tfidf[i][palabra];
            else matriz[i,j] = 0;
          }
      }
      return matriz;   
    }
   public static double [] SimCos (double [,] Matriz, double[] TFIDF_Query )
  {
    //formula de la similitud del coseno (para obtener el score por txt),
    // simCos = v1 * v2 / |v1|* |v2|
    //inicializando el array que va a almacenar el score (similitud del coseno) 
    double [] simcos = new double [Matriz.GetLength(0)];
    double moduloV1 = 0;
    //recorrer el tfidf de la query para obtener el modulo de cada parte de su vector
    for (int i = 0; i < TFIDF_Query.Length; i ++)
    {
      moduloV1 += Math.Sqrt(Math.Pow(TFIDF_Query[i], 2));
    }
    //con este for calculo la multiplicación de cada parte de los vectores
    for (int i = 0; i < Matriz.GetLength(0); i ++)
    {
      double V1V2 = 0;
      double moduloV2 = 0;
      for (int j = 0; j < Matriz.GetLength(1); j ++)
      {
        moduloV2 += Math.Sqrt(Math.Pow(Matriz[i,j], 2));
        V1V2 += Matriz[i,j] * TFIDF_Query[j] ;
      }
      simcos[i] = (float) V1V2 / (float) ((moduloV2 * moduloV1) + 0.00001);
    }
    return simcos;
  }
}