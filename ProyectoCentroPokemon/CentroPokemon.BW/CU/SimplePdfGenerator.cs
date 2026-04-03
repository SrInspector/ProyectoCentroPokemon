using System.Text;

namespace CentroPokemon.BW.CU;

internal static class SimplePdfGenerator
{
    public static byte[] Generate(string title, string body)
    {
        var safeTitle = Escape(title);
        var safeBody = Escape(body.Replace("\r", string.Empty));
        var stream = $"BT /F1 16 Tf 50 780 Td ({safeTitle}) Tj /F1 10 Tf 0 -30 Td ({safeBody.Replace("\n", ") Tj 0 -16 Td (")}) Tj ET";
        var pdf = new StringBuilder();
        pdf.AppendLine("%PDF-1.4");
        var offsets = new List<int>();
        void AddObject(string value)
        {
            offsets.Add(Encoding.ASCII.GetByteCount(pdf.ToString()));
            pdf.AppendLine($"{offsets.Count} 0 obj");
            pdf.AppendLine(value);
            pdf.AppendLine("endobj");
        }

        AddObject("<< /Type /Catalog /Pages 2 0 R >>");
        AddObject("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
        AddObject("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>");
        AddObject($"<< /Length {Encoding.ASCII.GetByteCount(stream)} >>\nstream\n{stream}\nendstream");
        AddObject("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
        var xrefPosition = Encoding.ASCII.GetByteCount(pdf.ToString());
        pdf.AppendLine("xref");
        pdf.AppendLine($"0 {offsets.Count + 1}");
        pdf.AppendLine("0000000000 65535 f ");
        foreach (var offset in offsets)
        {
            pdf.AppendLine($"{offset:D10} 00000 n ");
        }

        pdf.AppendLine("trailer");
        pdf.AppendLine($"<< /Size {offsets.Count + 1} /Root 1 0 R >>");
        pdf.AppendLine("startxref");
        pdf.AppendLine(xrefPosition.ToString());
        pdf.AppendLine("%%EOF");
        return Encoding.ASCII.GetBytes(pdf.ToString());
    }

    private static string Escape(string value)
        => value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
}
