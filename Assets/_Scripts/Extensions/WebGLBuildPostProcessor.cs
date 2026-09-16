#if UNITY_EDITOR
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class WebGLBuildPostProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

        string buildPath = report.summary.outputPath;
        string indexPath = Path.Combine(buildPath, "index.html");

        if (!File.Exists(indexPath))
        {
            UnityEngine.Debug.LogError("No se encontró index.html: " + indexPath);
            return;
        }

        long totalBytes = 0;
        string buildFolder = Path.Combine(buildPath, "Build");

        if (Directory.Exists(buildFolder))
        {
            foreach (string file in Directory.GetFiles(buildFolder, "*", SearchOption.AllDirectories))
            {
                totalBytes += new FileInfo(file).Length;
            }
        }

        double totalMB = totalBytes / (1024.0 * 1024.0);
        string totalString = totalMB.ToString("F2", CultureInfo.InvariantCulture);

        string html = File.ReadAllText(indexPath);
        html = Regex.Replace(
            html,
            @"var totalDownloadMB\s*=\s*[\d.]+;",
            $"var totalDownloadMB = {totalString};"
        );

        File.WriteAllText(indexPath, html);

        UnityEngine.Debug.Log($"<b>WebGL build final size:</b> {totalString} MB");
    }
}
#endif