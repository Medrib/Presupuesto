using Domain.Dtos.Cliente;

namespace Domain.Shared
{
    public class Functions
    {
        public static MesAño mesAñoIntParse(string MesAño)
        {
            int position = MesAño.IndexOf("-");

            return new MesAño() { 
                Mes = Int32.Parse(MesAño.Substring(position + 1)), 
                Año = Int32.Parse(MesAño.Substring(0, position))
            };
        }
    }
}
