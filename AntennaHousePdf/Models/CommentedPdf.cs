using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using System.Windows.Forms;
using AntennaHouseBusinessLayer.CommentExtraction;
using OfficeOpenXml;
using System.Text;
using System.IO;
using System.Drawing;
using OfficeOpenXml.Style;

namespace AntennaHousePdf.Models
{
    public class CommentedPdf
    {
        public List<HttpPostedFileBase> Pdf { get; set; }

        public byte[] buildCommentFile(string pdfFile)
        {
            List<Comment> comments = new List<Comment>();
            int item = 1;
            PdfReader myPdfReader = new PdfReader(pdfFile);
            for (int i = 0; i < myPdfReader.NumberOfPages; i++)
            {
                PdfDictionary pageDict = myPdfReader.GetPageN(i + 1);

                PdfArray annotArray = pageDict.GetAsArray(PdfName.ANNOTS);
                if (annotArray != null)
                {
                    for (int index = 0; index < annotArray.Size; index++)
                    {
                        PdfDictionary curAnnot = annotArray.GetAsDict(index);
                        PdfString contents = curAnnot.GetAsString(PdfName.CONTENTS);
                        if (!string.IsNullOrWhiteSpace(contents?.ToString()))
                        {
                            comments.Add(new Comment()
                            {
                                Item = item,
                                Page = i + 1,
                                PdfComment = contents.ToString()
                            });
                            item++;
                        }
                    }
                }
            }
            if (comments.Count > 0)
            {
                return buildExcelFile(comments);
            }
            else
            {
                throw new ArgumentException("There are no comments in this document");
            }
           
        }

        public byte[] buildExcelFile(List<Comment> comments)
        {
            ExcelPackage _package = new ExcelPackage(new MemoryStream());
            var ws1 = _package.Workbook.Worksheets.Add("Worksheet1");
            int rows = comments.Count + 1;
            string titleRange = "A1:G1";
            string BorderRange = "A1:C"+(comments.Count+1).ToString();
            var titleCells = ws1.Cells[titleRange];
            var AllCells = ws1.Cells[BorderRange];

            titleCells.Style.Font.Bold = true;
            titleCells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            titleCells.Style.Font.Size = 14;
            /*
            AllCells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            AllCells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            */
            ws1.Cells["A1"].Value = "Item No.";
            ws1.Cells["A1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws1.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            ws1.Cells["B1:C1"].Value = "Page";
            ws1.Cells["B1:C1"].Merge = true;
            ws1.Cells["B1:C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws1.Cells["B1:C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            ws1.Cells["D1:G1"].Value = "Comment";
            ws1.Cells["D1:G1"].Merge = true;
            ws1.Cells["D1:G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws1.Cells["D1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            int i = 2;
            
            foreach(Comment comment in comments)
            {
                ws1.Cells["A" + i+":A"+(i+1)].Value = comment.Item;
                ws1.Cells["A" + i + ":A" + (i + 1)].Merge = true;
                ws1.Cells["A" + i + ":A" + (i + 1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws1.Cells["A" + i + ":A" + (i + 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws1.Cells["A" + i + ":A" + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws1.Cells["A" + i + ":A" + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;



                ws1.Cells["B" + i + ":C"+(i+1)].Value = comment.Page;
                ws1.Cells["B" + i + ":C" + (i + 1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws1.Cells["B" + i + ":C" + (i + 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws1.Cells["B" + i + ":C" + (i + 1)].Merge = true;
                ws1.Cells["B" + i + ":C" + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws1.Cells["B" + i + ":C" + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                ws1.Cells["D" + i + ":G" + (i + 1)].Value = comment.PdfComment;
                ws1.Cells["D" + i + ":G" + (i + 1)].Style.WrapText = true;
                ws1.Cells["D" + i + ":G" + (i + 1)].Merge = true;
                ws1.Cells["D" + i + ":G" + (i + 1)].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                ws1.Cells["D" + i + ":G" + (i + 1)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws1.Cells["D" + i + ":G" + (i + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                i=i+2;
            }
            ws1.Column(1).Width = 15;
            return _package.GetAsByteArray();
        }
    }
}