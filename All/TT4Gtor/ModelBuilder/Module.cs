using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Extractor;
using NPOI.HSSF.Util;

namespace ModelBuilder
{
    #region Configuracion Modelo

    public class Entidad
    {
        private List<Atributo> atributos = new List<Atributo>();

        #region Propiedades

        public string Nombre { get; set; }
        public Tabla Tabla { get; set; }
       
        public List<Atributo> Atributos
        {
            get { return atributos; }
            set { atributos = value; }
        }

        #endregion

        public Campo ObtenerCamploClave()
        {
            foreach (Atributo atributo in this.Atributos)
                if (atributo.Campo.EsClavePrimaria)
                    return atributo.Campo;

            throw new Exception();
        }
    }

    public class Atributo
    {
        public string Nombre { get; set; }
        public Campo Campo { get; set; }
    }

    public class Tabla
    {
        public string Nombre { get; set; }
        public string Esquema { get; set; }

        public string ObternerNombreYEsquema()
        {
            return string.Format("[{0}].[{1}]", Esquema, Nombre);
        }
    }

    public class Campo
    {
        public string Nombre { get; set; }
        public string TipoDato { get; set; }
        public bool EsClavePrimaria { get; set; }
    }

    #endregion

    #region Datos de Prueba

    public static class DatosPrueba
    {
        public static List<Entidad> ObtenerEntidades()
        {
            List<Entidad> entidades = new List<Entidad>();

            Entidad entidadLocalidad = new Entidad { Nombre = "Proveedor", Tabla = new Tabla { Nombre = "Suppliers", Esquema = "Suppliers" } };
            
            entidadLocalidad.Atributos.Add(
                new Atributo { Nombre = "ID", 
                               Campo = new Campo { Nombre = "IDLocalidad", EsClavePrimaria = true, TipoDato = "INT" } 
                             }
            );

            entidadLocalidad.Atributos.Add(
                new Atributo
                {
                    Nombre = "Nombre",
                    Campo = new Campo { Nombre = "Nombre", EsClavePrimaria = false, TipoDato = "NVARCHAR(40)" }
                }
            );

            entidadLocalidad.Atributos.Add(
               new Atributo
               {
                   Nombre = "Codigo",
                   Campo = new Campo { Nombre = "Codigo", EsClavePrimaria = false, TipoDato = "NVARCHAR(40)" }
               }
           );

            entidades.Add(entidadLocalidad);

            return entidades;
        }

        public static List<Entidad> Leer()
        {
            string entidad = "PROVEEDOR";

            int? filaEncontrada;

            int? indiceHoja;

            List<Entidad> entidades = new List<Entidad>();
            
            Entidad entidadLocalidad = new Entidad { Nombre = "Proveedor", Tabla = new Tabla { Nombre = "Suppliers", Esquema = "Suppliers" } };

            HSSFWorkbook workbook = ExcelHelper.OpenSampleWorkbook("test.xls");

            indiceHoja = ExcelHelper.BuscarIndiceHoja(workbook, "Proveedor");

            if (indiceHoja != null)
            {
                var sheet = workbook.GetSheetAt(1);

                ExcelHelper.BuscarTextoEnHoja(sheet, 1, string.Format("ENTIDAD:{0}", entidad.ToUpper()), out filaEncontrada);

                if (filaEncontrada != null)
                {
                    entidadLocalidad = CrearEntidadDesdeHoja(entidadLocalidad, (HSSFSheet)sheet, (int)filaEncontrada);
                }

                entidades.Add(entidadLocalidad);

                return entidades;
            }

            throw new Exception(string.Format("No se encontro configuracion para la entidad {0}", entidad));
        }

        #region Entidad

        public static Entidad ParsearEntidad(Entidad entidadLocalidad, HSSFRow nombreAtributos, HSSFRow nombreCampos, HSSFRow tipoDatoCampos, HSSFRow esClavePrimariaCampos)
        {
            int columna = 1;

            while (nombreAtributos.GetCell(columna) != null)
            {
                entidadLocalidad.Atributos.Add(
                new Atributo
                {
                    Nombre = nombreAtributos.GetCell(columna).StringCellValue,
                    Campo = new Campo
                    {
                        Nombre = nombreCampos.GetCell(columna).StringCellValue,
                        EsClavePrimaria = (esClavePrimariaCampos.GetCell(columna).StringCellValue.ToUpper() == "SI" ? true : false),
                        TipoDato = tipoDatoCampos.GetCell(columna).StringCellValue
                    }
                }
                );

                columna++;
            }

            return entidadLocalidad;
        }

        public static Entidad CrearEntidadDesdeHoja(Entidad entidad, HSSFSheet hoja, int fila)
        {
            HSSFRow nombreAtributos;
            HSSFRow nombreCampos;
            HSSFRow tipoDatoCampos;
            HSSFRow esClavePrimariaCampos;

            nombreAtributos = hoja.GetRow(fila + 2);
            nombreCampos = hoja.GetRow(fila + 4);
            tipoDatoCampos = hoja.GetRow(fila + 6);
            esClavePrimariaCampos = hoja.GetRow(fila + 8);

            return ParsearEntidad(entidad, nombreAtributos, nombreCampos, tipoDatoCampos, esClavePrimariaCampos);
        }

        #endregion
    }

    #endregion
}
