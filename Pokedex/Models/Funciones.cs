using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Pokedex.Models.ViewModels;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace Pokedex.Models
{
    public class Funciones
    {

        /// <summary>
        /// Obtiene el formato de un archivo.
        /// </summary>
        /// <param name="cadena">Nombre completo del archivo con su formato.</param>
        /// <param name="caracter">Caracter que separa el nombre del formato.</param>
        /// <returns>El formato del archivo.</returns>
        /// 
        // Definir una función que recibe una cadena y un caracter, y devuelve la subcadena a la derecha del caracter
        public string GetFormato(string cadena, char caracter)
        {
            // Encontrar el índice del caracter en la cadena
            int indice = cadena.IndexOf(caracter);

            // Si el caracter se encuentra en la cadena
            if (indice != -1)
            {
                // Obtener la subcadena a la derecha del caracter
                // Sumamos 1 al índice para excluir el caracter
                // Restamos el índice al largo de la cadena para obtener la longitud de la subcadena
                string subcadena = cadena.Substring(indice + 1, cadena.Length - indice - 1);

                // Devolver la subcadena
                return subcadena;
            }
            else
            {
                // Devolver una cadena vacía
                return "";
            }
        }

        /// <summary>
        /// Obtiene la ruta de un archivo de imagen.
        /// </summary>
        /// <param name="localPath">La ruta local donde se guardan las imágenes.</param>
        /// <param name="destino">La carpeta dentro de la ruta local donde se quiere guardar la imagen.</param>
        /// <param name="nombre">El nombre de la imagen sin el formato.</param>
        /// <param name="formato">El formato de la imagen, como por ejemplo "png" o "jpg".</param>
        /// <returns>La ruta completa de la imagen.</returns>
        public string GetImage(string localPath, string destino, string nombre, string formato)
        {
            string name = Path.GetFileName($"{nombre}.{GetFormato(formato, '/')}");

            var filePath = Path.Combine(localPath, destino, name);

            return filePath;
        }


        private void AddPokemon(Pokemon pokemon, int a, int b)
        {
            var tipo1 = new PokemonsTipo
            {
                IdPokemonNavigation = pokemon,
                IdTipo = a
            };

            var tipo2 = new PokemonsTipo
            {
                IdPokemonNavigation = pokemon,
                IdTipo = b
            };

            if (tipo2.IdTipo == 0)
            {
                
            }
            else
            {
                
            }
        }
    }
}
