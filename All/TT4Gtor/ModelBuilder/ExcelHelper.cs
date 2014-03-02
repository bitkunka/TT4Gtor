
namespace ModelBuilder
{
    using System;
    using System.IO;
    using System.Text;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Model;
    using System.Collections.Generic;

    public class ExcelHelper
    {
        private static String TEST_DATA_DIR_SYS_PROPERTY_NAME = "HSSF.testdata.path";
        private static string _resolvedDataDir;
        private static bool _sampleDataIsAvaliableOnClassPath;

        public static Stream OpenSampleFileStream(String sampleFileName)
        {
            Initialise();

            if (_sampleDataIsAvaliableOnClassPath)
            {
                Stream result = OpenClasspathResource(sampleFileName);
                if (result == null)
                {
                    throw new Exception("specified test sample file '" + sampleFileName
                            + "' not found on the classpath");
                }
                //			System.out.println("Opening cp: " + sampleFileName);
                // wrap to avoid temp warning method about auto-closing input stream
                return new NonSeekableStream(result);
            }
            if (_resolvedDataDir == "")
            {
                throw new Exception("Must set system property '"
                        + TEST_DATA_DIR_SYS_PROPERTY_NAME
                        + "' properly before running tests");
            }


            if (!File.Exists(_resolvedDataDir+sampleFileName))
            {
                throw new Exception("Sample file '" + sampleFileName
                        + "' not found in data dir '" + _resolvedDataDir + "'");
            }

            
            //		System.out.println("Opening " + f.GetAbsolutePath());
            try
            {
                return new FileStream(_resolvedDataDir+sampleFileName,FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                throw;
            }
        }

        private static void Initialise()
        {
            String dataDirName = Properties.Settings.Default.HSSFtestdatapath;       

            if(dataDirName=="")
                    throw new Exception("Must set system property '"
                            + TEST_DATA_DIR_SYS_PROPERTY_NAME
                            + "' before running tests");

            if (!Directory.Exists(dataDirName))
            {
                throw new IOException("Data dir '" + dataDirName
                        + "' specified by system property '"
                        + TEST_DATA_DIR_SYS_PROPERTY_NAME + "' does not exist");
            }
            _sampleDataIsAvaliableOnClassPath = true;
            _resolvedDataDir = dataDirName;
        }

        private static Stream OpenClasspathResource(String sampleFileName)
        {
          
            
            //FileStream file = new FileStream(System.Configuration.ConfigurationSettings.AppSettings["HSSF.testdata.path"] + sampleFileName, FileMode.Open);

            FileStream file = new FileStream(Properties.Settings.Default.HSSFtestdatapath + sampleFileName, FileMode.Open);
            return file;
        }

        private class NonSeekableStream : Stream
        {

            private Stream _is;

            public NonSeekableStream(Stream is1)
            {
                _is = is1;
            }

            public int Read()
            {
                return _is.ReadByte();
            }
            public override int Read(byte[] b, int off, int len)
            {
                return _is.Read(b, off, len);
            }
            public bool markSupported()
            {
                return false;
            }
            public override void Close()
            {
                _is.Close();
            }
            public override bool CanRead
            {
                get { return _is.CanRead; }
            }
            public override bool CanSeek
            {
                get { return false; }
            }
            public override bool CanWrite
            {
                get { return _is.CanWrite; }
            }
            public override long Length
            {
                get { return _is.Length; }
            }
            public override long Position
            {
                get { return _is.Position; }
                set { _is.Position = value; }
            }
            public override void Write(byte[] buffer, int offset, int count)
            {
                _is.Write(buffer, offset, count);
            }
            public override void Flush()
            {
                _is.Flush();
            }
            public override long Seek(long offset, SeekOrigin origin)
            {
                return _is.Seek(offset, origin);
            }
            public override void SetLength(long value)
            {
                _is.SetLength(value);
            }
        }

        public static HSSFWorkbook OpenSampleWorkbook(String sampleFileName)
        {
            try
            {
                return new HSSFWorkbook(OpenSampleFileStream(sampleFileName));
            }
            catch (IOException)
            {
                throw;
            }
        }
       
        public static HSSFWorkbook WriteOutAndReadBack(HSSFWorkbook original)
        {

            try
            {
                MemoryStream baos = new MemoryStream(4096);
                original.Write(baos);
                return new HSSFWorkbook(baos);
            }
            catch (IOException)
            {
                throw;
            }
        }

        public static byte[] GetTestDataFileContent(String fileName)
        {
            MemoryStream bos = new MemoryStream();

            try
            {
                Stream fis = ExcelHelper.OpenSampleFileStream(fileName);

                byte[] buf = new byte[512];
                while (true)
                {
                    int bytesRead = fis.Read(buf,0,buf.Length);
                    if (bytesRead < 1)
                    {
                        break;
                    }
                    bos.Write(buf, 0, bytesRead);
                }
                fis.Close();
            }
            catch (IOException)
            {
                throw;
            }
            return bos.ToArray();
        }

        #region Buscar 

        public static void BuscarTextoEnHoja(HSSFSheet hoja, int columna, string textoBuscado, out int? filaEncontrada)
        {
            filaEncontrada = null;

            HSSFRow filaActual;

            string textoActual;

            for (var fila = 1; fila <= 65535; fila++)
            {
                if (hoja.GetRow(fila) != null)
                {
                    filaActual = hoja.GetRow(fila);

                    if (filaActual.GetCell(columna) != null)
                    {
                        textoActual = filaActual.GetCell(columna).StringCellValue;

                        if (textoActual == textoBuscado)
                        {
                            filaEncontrada = fila;
                            continue;
                        }
                    }
                }
            }
        }

        public static int? BuscarIndiceHoja(HSSFWorkbook libro, string nombreHoja)
        {
            var sheets = new List<Sheet>();

            for (int i = 0; i < libro.NumberOfSheets; i++)
            {
                if (libro.GetSheetName(i).ToUpper() == nombreHoja.ToUpper())
                {
                    return i;
                }
            }

            return null;

        }

        #endregion
    }
}