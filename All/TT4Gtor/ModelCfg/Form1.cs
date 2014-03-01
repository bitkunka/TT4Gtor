using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Extractor;
using NPOI.HSSF.Util;
using ModelBuilder;

namespace ModelCfg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Leer("Localidad");

            List<Entidad> entidades = DatosPrueba.Leer();
        }
    }
}
