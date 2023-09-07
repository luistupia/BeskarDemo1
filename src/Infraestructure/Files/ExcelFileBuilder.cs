using System.Drawing;
using System.Reflection;
using Application.Common.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Attributes;
using OfficeOpenXml.Style;

namespace Infraestructure.Files;

internal class ExcelFileBuilder : IExcelFileBuilder
{
    public byte[] GetStreamFileExcel<T>(IEnumerable<T>? records, string sheetName = "")
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var stream = new MemoryStream();
        var rowStart = 1;
        
        using (var package = new ExcelPackage(stream))
        {
            var workSheet = package.Workbook.Worksheets.Add(string.IsNullOrEmpty(sheetName) ? "Hoja1" : sheetName);
            
            MemberInfo[] membersToInclude = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !Attribute.IsDefined(p, typeof(EpplusIgnore)))
                .ToArray();

            for (var i = 0; i < membersToInclude.Length; i++)
            {
                var prop = membersToInclude[i];
                
                //Si la propiedad es tipo fecha, cambia a estilo fecha en el excel la columna
                if (prop.MemberType == MemberTypes.Property && (((PropertyInfo)prop).PropertyType == typeof(DateTime)
                    || ((PropertyInfo)prop).PropertyType == typeof(DateTime?)))
                    workSheet.Column(i + 1).Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                workSheet.Cells[rowStart, i + 1].Value = prop.Name;
                workSheet.Column(i + 1).Style.Font.Size = 8.5F;
                workSheet.Column(i + 1).Style.Font.Name = "Tahoma";
            }

            //Dar formato a la cabecera de la hoja
            using (var rng = workSheet.Cells[rowStart, 1, rowStart, membersToInclude.Length])
            {
                rng.AutoFilter = true;
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rng.Style.Font.Color.SetColor(Color.White);
                rng.Style.Fill.BackgroundColor.SetColor(Color.Black);
            }

            //Registro los datos en la hoja
            workSheet.Cells[$"A{rowStart + 1}"].LoadFromCollection(records, false, null,
                BindingFlags.Instance | BindingFlags.Public,
                membersToInclude);

            workSheet.View.FreezePanes(rowStart + 1, 1);
            package.Save();
        }
        
        //Retorna el archivo convertido en array de bytes
        stream.Position = 0;
        return stream.ToArray();
    }
}